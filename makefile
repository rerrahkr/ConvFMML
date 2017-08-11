# makefile for ConvFMML

ConvFMML.exe: convfmml.o config.o input.o analyze.o modify.o output.o
	gcc -Wall -W -O3 convfmml.o config.o input.o analyze.o modify.o output.o -o ConvFMML

.c.o:
	gcc -c $<

convfmml.o: convfmml.c convfmml.h config.h input.h analyze.h modify.h output.h
config.o:   config.c convfmml.h config.h
input.o:    input.c convfmml.h input.h analyze.h
analyze.o:  analyze.c convfmml.h analyze.h
modify.o:   modify.c convfmml.h modify.h
output.o:   output.c convfmml.h output.h
