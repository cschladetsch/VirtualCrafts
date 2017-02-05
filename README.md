# VirtualCrafts
This will be a virtual RC system, supportig Quads, Fixed-wing planes, and a separate Transmitter app to run on mobile phones in leui of a real conroller.

A goal is to eventually make the game playable as a VR demo, either Line of Sight with the vehicles or First Person. Mechanics such as racing and accuracy can be added after the basic framework is practical.

Pysical accuracy in the modelling is a clear goal with all correct sizes, weights, forces, springs, and torque values being used throughout. Destruction of Crafts is definiately a goal, with a view to allowing the player to attempt to fix and adjust broken models.

There will be a common framework made, to support creation of:

* Quadcopters
* Tricopters
* Helicopters
* Fixed-wing 2,3 and 4-channel planes
* Cars and monster Trucks
* Maybe even Boats and other things...

## Transmitter
The Transmitter is a separate app designed to work on either Android or iOS. The player holds the device in landscape mode with two simulated sticks for THR/RUD and ELE/AIL.

The controller binds to the virtual crafts and after that supplies input data to the flight controllers on them.

Of course, it would be nice to support actual transmitters plugged into computers, but that's a difficult task itself with many proprietray standards and lack of documentation from manufacturers.

## Technical Risks
This is a non-trivial project that will take a few months at least to get somewhat playable with 1-2 craft and a simple Transmitter phone app.

# Road Map

1. Research
1. Math libraries: PID controllers, low-pass filters, etc.
1. Get Transmitter working
  1. Bootstrapping PID tuning
  1. Channel mixing
  1. Expos and rates
1. Get Working Quad
1. Get Working Fixed-Wing
1. Get Working Car
1. Get Working Truck

Overall, I see this taking about 200-400hrs over 1-2 years.

Please contact me for questions or suggestions.

# Multiple Instances of Unity
From the top-level of the project:

	$ tx # opens the Transmitter project in Unity
	
	$ rx # opens (in parallel) the Receiver project
	
	$ cr # opens the Crafts project in Unity

You can do all of these at once.