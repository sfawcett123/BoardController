# TCP/IP handler for board mulitplexing

[![Deploy](https://github.com/sfawcett123/BoardController/actions/workflows/main.yml/badge.svg)](https://github.com/sfawcett123/BoardController/actions/workflows/main.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sfawcett123_BoardController&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=sfawcett123_BoardController)
[![codecov](https://codecov.io/gh/sfawcett123/BoardController/branch/main/graph/badge.svg?token=2D6BX22N6Q)](https://codecov.io/gh/sfawcett123/BoardController)

## Purpose
when building the listener for the Microsoft Flight Simulator (MFS) 
I wanted to allow TCP/IP connections to the interface on a various ports, for registered boards.

This librray provides a function to communicate with various ports, at the moment there is no security
as the data is not meant to be confidential and the purpose of the application is within its own private network.


