# Async Sockets with Unity3d

First attempts just hang the editor at the first call to:

	var asyncResult = _listener.BeginAccept( new AsyncCallback(AcceptCallback), _listener);

This also happens in a stand-alone build.

## Attempt #2

Try to get it working as a stand-alone .net assembly and link that to unity. Unsure if that will help.

Maybe the .Net Mono framework will play nicer with the Unity editor as a plugin assembly. Unsure.

## Attempt #3

Try to get it working as a stand-along C++ assembly and link to that in unity. 

At last at this point there's no shared runtime between Unity3d and the plugin.

If that doesn't help then I'm kinda out of ideas.

