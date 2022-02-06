# Realtime Hand

Unity package to track a hand in realtime!

## Sample Video

https://user-images.githubusercontent.com/674951/152693534-cf32f285-c99c-4f66-acd8-c1d583603b19.mov


## Features
* 60 FPS hand detection
* 3D Bones world detection

## Requirements
* Unity 2021.2+
* ARFoundation
* iPhone with Lidar support

## Installation
* Add the package `RealtimeHand `to your manifest
* Add the `SwiftSupport`package to enable swift development
* Configure 

Check the ``RealtimeHandSamplee``for details

## Classes

### `RTHand.Joint`
* `screenPos`: 2D position in normalized screen coordinates;
* `texturePos`: 2D position in normalized CPU image coordinates;
* `worldPos`: 3D position in worldpsace
*  `name`: name of the joint, matching the native one
* `distance`: distance from camera in meter
* `isVisible`: if the joint has been identified (from the native pose detection)
*`confidence`: confidence of the detection

### `RTHand.RealtimeHandManager`
Do most of the heavy work for you : just add it to your project, and subscribe to the ``HandUpdated` event to be notified when a hand pose has been detected

Steps:
* Create a GameObject
* Add the `RealtimeHandManager`component
* Configure it with the `ARSession`, `ARCameraManager`, `AROcclusionManager`objects
* Subscribe to  `Action<RealtimeHand> HandUpdated;`to be notified

>  OcclusionManager must be configured with `temporalSmoothing=Off`and
> `mode=fastest`for optimal result


### `RTHand.RealtimeHand`
If you want to have a full control on the flow, you can manually intialize and call the hand detection process : more work, but more control. 

#### Properties
* `IsInitialized` : to check if the object has been properly initialized (ie: the ARSession has been retrieved)
* `IsVisivle`: to know if the hand is currently visible or not
* `Joints`: dictionary of the all the joints 

#### Functions
* ``Initialize(ARSession  _session, ARCameraManager  _arCameraManager, Matrix4x4  _unityDisplayMatrix)``:  initialize the object with the required components
>  The session must be in tracking mode 
* ``Dispose()``:  release the component and it resources
*  `Process( CPUEnvironmentDepth  _environmentDepth, CPUHumanStencil  _humanStencil ) `: launch the detection method using the depth buffers

Check the ``RealtimeHandManager``as an example


## Under the hood
When a camera frame is received :
* Execute synchronously `VNDetectHumanHandPoseRequest` to retrieve a 2D pose estimation from the OS
* Retrieve the `environmentDepth`and `'humanStencil`CPU images 
* From the 2D position of each bone, extract its 3D distance using the depth images to reconstruct a 3D position

## References
* Linkedin Post

## Revision
1.0.0  Initial Release
