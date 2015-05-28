Part 2
------

A very simple client-server chat with no real error handling, but otherwise proper TCP. The server handles multiple simultaneous chat connections, and broadcasts all the messages to all connected clients.

Newly introduced TCP and networking concepts:
* Persistent bi-directional communication channel
* Improved message framing - all messages are prefixed with length, and include a message identifier
* Graceful TCP shutdown on both the client and server
* Handling state for multiple simultaneous clients - login, unique names for users, unique name used for messages
* Broadcasting messages to TCP clients
 
Again, more error handling is needed for any real application - connection, reading and writing can all fail, 
and neither the server nor the client are resistant to malicious data.