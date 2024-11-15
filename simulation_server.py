from flask import Flask, jsonify
from flask_cors import CORS
import threading
import time

# Importa tus modelos y agentes de MESA
from mesa import Model
from mesa.time import RandomActivation
from mesa.space import MultiGrid

# Aquí define tus clases CarAgent, SemaphoreAgent y CityModel como en el código anterior.
# Por simplicidad, usaré un modelo básico para la demostración.

class CityModel(Model):
    def __init__(self, width, height, num_cars):
        self.grid = MultiGrid(width, height, True)
        self.schedule = RandomActivation(self)
        self.running = True

        # Add car agents (simplified for the example)
        for i in range(num_cars):
            car = CarAgent(i, self)
            self.grid.place_agent(car, (i, i))  # Initial positions
            self.schedule.add(car)

    def step(self):
        self.schedule.step()

class CarAgent(mesa.Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.pos = (0, 0)

    def step(self):
        # Dummy movement logic (update as needed)
        x, y = self.pos
        self.pos = (x + 1, y + 1)

# Create Flask app
app = Flask(__name__)
CORS(app)  # Allows Unity to access the API

# Global variables for the simulation
model = CityModel(25, 25, 3)  # Grid size: 25x25, 3 cars

# Run the simulation in a separate thread
def simulation_loop():
    while model.running:
        model.step()
        time.sleep(1)  # Adjust the simulation speed

# Start the simulation thread
thread = threading.Thread(target=simulation_loop)
thread.daemon = True
thread.start()

# API Endpoint to get simulation data
@app.route('/get_simulation_data', methods=['GET'])
def get_simulation_data():
    car_data = [
        {"id": agent.unique_id, "position": agent.pos}
        for agent in model.schedule.agents if isinstance(agent, CarAgent)
    ]
    return jsonify({"cars": car_data})

if __name__ == '__main__':
    app.run(host='127.0.0.1', port=5000)
