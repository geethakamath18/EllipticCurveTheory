# DOS-2020
Distrubuted Operating Systems Fall 2020
The main objective of this project was to find all numbers between 1 and N within the provided range. This project has been implemented in F# using the AKKA.NET framework

Group Members:
Geetha Kamath Koteshwar(UFID: 7012-8912, eMail: gkamathkoteshwar@ufl.edu)
Keerthi Suresh (UFID: 4701-4942 , eMail: keerthi.suresh@ufl.edu)

1. Steps to run the program:
    1) Change the directory as the root of the project with the proj1.exs file.
    2) Run the following command: 
        dotnet fsi --langversion:preview project1.fsx arg1 arg2
        where arg1 is the value of N and arg2 is the value of k

2. We decided to use the number of logical processors as a metric to determine how many actors would work on a given problem. Based on the size of the number N, the work is evenly split between the actors. For a machine with 12 logical processors, 24000 actors are spawned. 

3. We performed experiments to determine the number of actors to spawn. We experimented on numbers ranging from 120 to 120,000 on a machine with 12 logical processors. We determined that a number as high as 240,00 would cause the concurrency to fall and the ratio of CPU time to Real time was ~1.5. When we tested the system with 120 actors, ratio of CPU time to Real time was ~1.6. On testing with 24000 actors, the system yielded a ratio of CPU time to Real time was ~2.5. Therefore we chose this to be the ideal number.

4. For the given input of N=10^6 and k=4, we do not get any output, but the running time is as follows:
    Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
    Real: 00:00:10.727, CPU: 00:00:26.406, GC gen0: 118, gen1: 15, gen2: 1
    The ratio of CPU time to Real time is 2.461
  
5. The largest input we provided to the program was N=10^8 and k=24
    The following is the output for the same:
    1 
    9
    20
    25
    44
    76
    121
    197
    304
    353
    540 
    856
    1301
    2053
    3112
    3597
    5448
    8576
    12981
    20425
    30908
    35709
    54032
    84996
    128601
    202289
    306060 
    353585
    534964 
    841476 
    1273121 
    2002557 
    3029784 
    3500233
    5295700
    8329856
    12602701
    19823373
    29991872
    34648837
    45863965
    52422128
    82457176

    The CPU and real time is as follows:
    Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0 of the 
    Real: 00:00:13.595, CPU: 00:00:31.968, GC gen0: 118, gen1: 15, gen2: 1
    The ratio of CPU time ot Real time is 2.35