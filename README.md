# TCP/IP handler for board mulitplexing
## Purpose
when building the listener for the Microsoft Flight Simulator (MFS) 
I wanted to allow TCP/IP connections to the interface on a various ports, for registered boards.

This librray provides a function to communicate with various ports, at the moment there is no security
as the data is not meant to be confidential and the purpose of the application is within its own private network.

## Methods
The main methods are:

- Add - Adds a board to the list of registered boards.
- Remove_Timed_out - Removes any boards from the list that have not responded.
- 