# InProgress 

- move ship parameters to common project settings (ship size, thrusters power, thermal emissions etc.)
- introduce ship type on server
- fog of war:
	- do(do not) propagate ships based on ship parameters (both own and neighbor)

# ToDo
- display ship systems on screen
    - show ship schematics
    - show thrusters 
        - main and side
        - in red when active

# Debt
- *[InterierBuilder]* generate upper, lover corners
- *[InterierBuilder]* generate middle walls
- Highlite interacable objects (animate interaction)
- *[PilotSeat]* Update screen(s) layout
- *[PilotSeat]* change pilot seat screens (???)

# Done
- avoid propagating duplicate states from different sectors
- handle disconnects:
	- Handle in-activity on a client (if neighbor not updated for some time drop them)
	- Handle connection loss on server. ( notify neighbors)
	- Propagate neighbor leaving neighborhood
- display other player ships
- fisheye camera
- display ship position on screen
- control the ship
- *[PilotSeat]* control systems based on player aim
- *[refactor]* extract ship systems into separate nodes, access those via standart search mechanics
- character animations
- door with animations
- bug camera clips into walls
- ship interior made from grid map
- player sceleton
- pilot seat model
- show engine power setting on a screen
- player controls the ship. Step in particular place in interier (control seat), and then controls the thrusters etc.
- make ship exterier prototype
- create complex ship interior
- camera moves with player in interor mode
- make ship interier prototype
- switch between interier and exterier views