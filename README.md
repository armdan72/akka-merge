# akka-merge
Demo about merging streams of event sourcing events
The 2 databases should be in c:\tmp folder, and they contain 1 event each.
Running the console project 3 files will be created in c:\tmp: merge1.txt with the timestamp from db1, merge2.txt with the timestamp from db2, and merge.txt that should contain both timestamp but it contains the same duplicated twice.
