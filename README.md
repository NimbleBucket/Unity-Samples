# Unity-Samples
Various examples of my coding style/ability



# Gravity System


The gravity system I have created is for a 3d platformer, with non-realistic physics. The system can be best described by the two interfaces: IGravity and IGravityProvider.

IGravity is meant to be implemented on anything that can be affected by gravity. It includes an extension method call PollDesiredGravity in the same file (made prior to default interface methods) that you can call to try and find an alternate source of gravity. This will return a gravity direction and scale that your class can use however it wants. I created two different classes that implement IGravity: ObjectGravity and CharacterGravity. ObjectGravity is very simple in that it takes the gravity direction and scale and just applies that to a source rigidbody, but the CharacterGravity is more complicated and it will prioritize changing the gravity on a character in a way that I have found to be easier for players to predict/track.

IGravityProvider is implemented on any gravity source and includes methods for taking a position and turning that into a gravity direction and scale, as well as a way to determine whether or not the IGravityProvider can supply gravity to an object. The second one is useful for my SplineGravity because it uses a convex mesh to detect the gravity zone, but I use the CanSupplyGravity method to check if the player is within a box on the closest point on the spline.


# Damage System
The Damage System has two major parts: DamageTrigger and IDamageable.

DamageTrigger determines how/when/what type of damage is applied and how the direction/intensity of force that would get applied to the thing being damaged. I made an interface called IPushCalculator to determine the direction/intensity that the damaged item will be pushed and used the SerializeReference attribute and Odin inspector to allow you to assign this field in the inspector. I used Odin inspector to customize the inspector visibility of some elements to only show when they are relevant.

IDamageable is an interface that you can implement to add to determine the health, effective damage types, and how push forces are applied to the object. I declared all damage types in an enum in this interface using the [Flags] attribute so that damage can be of multiple types at the same time.