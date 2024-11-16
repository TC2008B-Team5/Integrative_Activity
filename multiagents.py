from mesa import Model, Agent
from mesa.time import RandomActivation
from mesa.space import MultiGrid
from mesa.visualization.modules import CanvasGrid
from mesa.visualization.ModularVisualization import ModularServer
import random

# Definición de direcciones de tráfico y espacios de estacionamiento
TRAFFIC_DIRECTIONS = {
    (1, 2): "down", (1, 3): "down", (1, 4): "down",
    (2, 4): "right", (3, 4): "right",
    # Agrega más direcciones de tráfico según sea necesario
}

PARKING_SPOTS = {
    1: (9, 10), 2: (12, 15), 3: (18, 20), 4: (5, 6),
    # Agrega más lugares de estacionamiento según sea necesario
}

OBSTACLES = [
    (5, 5), (15, 15), (10, 10), (12, 14),
    # Agrega más obstáculos si es necesario
]

class CarAgent(Agent):
    def __init__(self, unique_id, model, start_parking, target_parking):
        super().__init__(unique_id, model)
        self.start_parking = start_parking
        self.target_parking = target_parking
        self.parked = False

    def step(self):
        if self.parked:
            return  # No se mueve si ya está estacionado

        # Verificar semáforo
        current_semaphore = self.model.get_semaphore(self.pos)
        if current_semaphore and current_semaphore.color == "red":
            return  # Espera en la luz roja

        # Mover según las direcciones de tráfico
        if not self.move_according_to_traffic():
            return  # Si no puede moverse, espera

        # Verificar si llegó al estacionamiento de destino
        if self.pos == self.target_parking:
            print(f"Car {self.unique_id} parked at {self.target_parking}")
            self.parked = True

    def move_according_to_traffic(self):
        if self.pos not in TRAFFIC_DIRECTIONS:
            return False  # No hay dirección de tráfico para esta celda

        direction = TRAFFIC_DIRECTIONS[self.pos]
        possible_move = None

        if direction == "up":
            possible_move = (self.pos[0] - 1, self.pos[1])
        elif direction == "down":
            possible_move = (self.pos[0] + 1, self.pos[1])
        elif direction == "left":
            possible_move = (self.pos[0], self.pos[1] - 1)
        elif direction == "right":
            possible_move = (self.pos[0], self.pos[1] + 1)

        if possible_move and self.model.is_cell_navigable(possible_move):
            self.model.grid.move_agent(self, possible_move)
            return True
        return False

class SemaphoreAgent(Agent):
    def __init__(self, unique_id, model, pos):
        super().__init__(unique_id, model)
        self.color = "green"
        self.change_interval = 5  # Cambia de luz cada 5 pasos
        self.pos = pos

    def step(self):
        if self.model.schedule.time % self.change_interval == 0:
            self.color = "red" if self.color == "green" else "green"

class CityModel(Model):
    def __init__(self, width, height, num_cars):
        self.grid = MultiGrid(width, height, True)
        self.schedule = RandomActivation(self)

        # Agregar semáforos
        semaphore_positions = [(8, 5), (17, 3), (7, 9), (15, 10)]
        for i, pos in enumerate(semaphore_positions):
            semaphore = SemaphoreAgent(f"S{i}", self, pos)
            self.grid.place_agent(semaphore, pos)
            self.schedule.add(semaphore)

        # Agregar autos
        for i in range(num_cars):
            start, target = random.sample(list(PARKING_SPOTS.keys()), 2)
            car = CarAgent(i, self, PARKING_SPOTS[start], PARKING_SPOTS[target])
            self.grid.place_agent(car, PARKING_SPOTS[start])
            self.schedule.add(car)

        # Agregar obstáculos
        for i, pos in enumerate(OBSTACLES):
            obstacle = Agent(f"B{i}", self)
            self.grid.place_agent(obstacle, pos)

    def step(self):
        self.schedule.step()

    def get_semaphore(self, pos):
        for agent in self.grid.get_cell_list_contents(pos):
            if isinstance(agent, SemaphoreAgent):
                return agent
        return None

    def is_cell_navigable(self, pos):
        if not (0 <= pos[0] < self.grid.width and 0 <= pos[1] < self.grid.height):
            return False
        contents = self.grid.get_cell_list_contents(pos)
        return all(not isinstance(agent, Agent) for agent in contents)

# Visualización
def agent_portrayal(agent):
    if isinstance(agent, CarAgent):
        color = "blue" if not agent.parked else "lightblue"
        return {"Shape": "circle", "Color": color, "r": 0.8, "Layer": 1}
    elif isinstance(agent, SemaphoreAgent):
        color = "green" if agent.color == "green" else "red"
        return {"Shape": "rect", "Color": color, "w": 1, "h": 1, "Layer": 0}
    else:
        return {"Shape": "rect", "Color": "gray", "w": 1, "h": 1, "Layer": 0}


grid = CanvasGrid(agent_portrayal, 25, 25, 500, 500)
server = ModularServer(CityModel, [grid], "City Traffic Simulation", {"width": 25, "height": 25, "num_cars": 3})

server.port = 8521
server.launch()

#Hola miren les explico el estado:
#Los multiagentes funcionan en este código de multiagents.py
#Se comunican con el unity mediante Flask, el código que funcionará como Backend con Flask es simulation_server.py
#Lo que hace este código:
#Flask App: Crea un servidor que Unity puede consultar.
#Endpoint /get_simulation_data: Devuelve las posiciones de los agentes en formato JSON.
#Simulación en Hilo: La simulación MESA corre en un hilo separado para que el servidor Flask siga respondiendo a las solicitudes.
#Después, se utilizará un código C# En Unity para hacer solicitudes al servidor Flask y procesar los datos, se llama FlaskConnector.cs
#Lo que hace este código:
#Fetch Data: Realiza una solicitud GET al endpoint Flask /get_simulation_data.
#Deserialize JSON: Convierte la respuesta JSON en objetos C# utilizables.
#Update Car Positions: Actualiza las posiciones de los coches en Unity.
#Recomendaciones de ChatGPT:
#Car Prefab:
#Crea un GameObject en Unity (por ejemplo, un cubo o modelo 3D) y asígnalo como carPrefab en el script FlaskConnector.
#Asegúrate de Tener Newtonsoft.Json:
#Instala el paquete Newtonsoft.Json para Unity desde NuGet o el Unity Asset Store.
#Ubicación del Código:
#Coloca simulation_server.py en la misma carpeta o accesible desde tu proyecto en Unity. No es necesario que esté en el mismo directorio, pero deben poder comunicarse.

#Ayúdenme a darle las coordenadas para que se mueva por donde debe el carro (como en el maze)
#En la visualización de Mesa se debe ver lo que está pasando (ya esta funcionando la visualización pero no muestra nada)
#La visualización puede verse así:
#Blue cars moving toward their target.
#Light blue cars once parked.
#Red/green semaphores switching states.
#Gray obstacles representing buildings.
#Hay que verificar que los semáforos están funcionando correcta y sincronizadamente, además de que en Unity visualmente si los carros se están moviendo conforme dice el código, respetando los semáforos y evitando obstáculos
#