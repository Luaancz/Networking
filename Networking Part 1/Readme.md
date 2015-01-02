Part 1
------

A very simple client-server example with no real error handling, but otherwise proper TCP. The server handles a simple Echo (i.e. repeats what you sent to it) in a way similar to HTTP - the client opens a new TCP connection, sends its data and waits for data and a graceful shutdown by the server.

Even this simple example already demonstrates several key TCP and networking concepts:
* Basic asynchronous socket client and server operation
* Message framing - in this case, the client sends the length of its request
* Graceful TCP shutdown - a simple Close on the server, the client receives a zero-byte "read" when this happens
* Message fragmentation and relevant issues - this goes hand to hand with message framing; TCP doesn't guarantee you that a single write results in a single read on the server or vice versa. Ignoring this is often the cause of TCP code that "usually works".
 
More error handling is of course necessary for serious reliable applications, but in a TCP communication like this, there is actually very little that could go horribly wrong.
