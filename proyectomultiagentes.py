# -*- coding: utf-8 -*-
"""ProyectoMultiagentes.ipynb

Automatically generated by Colab.

Original file is located at
    https://colab.research.google.com/drive/1tUGxPjouWLtJe3bA0hU0PbrTbD80fn61
"""

!pip install mesa==0.8.9
!pip install nest_asyncio

import nest_asyncio
nest_asyncio.apply()
import mesa
from mesa.space import MultiGrid
from mesa.time import RandomActivation
import random
from mesa.visualization.modules import CanvasGrid
from mesa.visualization.ModularVisualization import ModularServer
import matplotlib.pyplot as plt


# Define traffic directions for each cell, based on a grid structure
# Example: {(x, y): "up"/"down"/"left"/"right"}
# This is a simplified representation; you'll need to define the direction for each street cell.
TRAFFIC_DIRECTIONS = {
    (1, 2): "down", (1, 3): "down", (1, 4): "down",
    (2, 4): "right", (3, 4): "right",
    # Define all other directions based on your grid setup.
}

class CarAgent(mesa.Agent):
    def __init__(self, unique_id, model, start_parking, target_parking):
        super().__init__(unique_id, model)
        self.start_parking = start_parking
        self.target_parking = target_parking
        self.reached_target = False

    def step(self):
        if not self.reached_target:
            current_semaphore = self.model.get_semaphore(self.pos)
            if current_semaphore and current_semaphore.color == "red":
                return

            if not self.move_according_to_traffic():
                # If no valid moves according to traffic, wait
                return

            if self.pos == self.target_parking:
                print(f"Car {self.unique_id} reached target parking {self.target_parking}")
                self.reached_target = True

    def move_according_to_traffic(self):
        if self.pos not in TRAFFIC_DIRECTIONS:
            return False

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

        if possible_move and self.model.grid.is_cell_empty(possible_move):
            self.model.grid.move_agent(self, possible_move)
            return True
        return False

class SemaphoreAgent(mesa.Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.color = "green"
        self.change_interval = 5  # Change light every 5 steps

    def step(self):
        if self.model.schedule.time % self.change_interval == 0:
            self.color = "red" if self.color == "green" else "green"

class BuildingAgent(mesa.Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)

class CityModel(mesa.Model):
    def __init__(self, width, height, num_cars):
        self.grid = MultiGrid(width, height, True)
        self.schedule = RandomActivation(self)

        self.parking_spots = {
            1: (9, 10), 2: (2, 4), 3: (18, 20), 4: (12, 10),
            5: (20, 20), 6: (6, 4), 7: (9, 8), 8: (22, 22)
        }

        semaphore_positions = [(8, 5), (17, 3), (7, 9), (15, 10)]

        for i, pos in enumerate(semaphore_positions):
            semaphore = SemaphoreAgent(f"S{i}", self)
            self.grid.place_agent(semaphore, pos)
            self.schedule.add(semaphore)

        for i in range(num_cars):
            start, target = random.sample(list(self.parking_spots.keys()), 2)
            car = CarAgent(i, self, self.parking_spots[start], self.parking_spots[target])
            self.grid.place_agent(car, self.parking_spots[start])
            self.schedule.add(car)

        building_positions = [
            (5, 5), (15, 15), (10, 10), (12, 15),
        ]
        for i, pos in enumerate(building_positions):
            building = BuildingAgent(f"B{i}", self)
            self.grid.place_agent(building, pos)

    def step(self):
        self.schedule.step()

    def get_semaphore(self, pos):
        for agent in self.grid.get_cell_list_contents(pos):
            if isinstance(agent, SemaphoreAgent):
                return agent
        return None

# Visualization setup for MESA
from mesa.visualization.modules import CanvasGrid
from mesa.visualization.ModularVisualization import ModularServer

def agent_portrayal(agent):
    if isinstance(agent, CarAgent):
        color = "blue" if not agent.reached_target else "lightblue"
        return {"Shape": "circle", "Color": color, "r": 0.8, "Layer": 1}
    elif isinstance(agent, SemaphoreAgent):
        color = "green" if agent.color == "green" else "red"
        return {"Shape": "rect", "Color": color, "w": 1, "h": 1, "Layer": 0}
    elif isinstance(agent, BuildingAgent):
        return {"Shape": "rect", "Color": "gray", "w": 1, "h": 1, "Layer": 0}

grid = CanvasGrid(agent_portrayal, 25, 25, 500, 500)
server = ModularServer(CityModel, [grid], "City Traffic Simulation", {"width": 25, "height": 25, "num_cars": 3})
server.port = 8525
server.launch()

#Friday, the car should move according to the traffic direction/arrows
#Mesa visualization where we can see when a car reaches its destination
#In case the semaphores area already correctly