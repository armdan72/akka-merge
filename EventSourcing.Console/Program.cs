// See https://aka.ms/new-console-template for more information
using Akka.Actor;
using EventSourcing;
using EventSourcing.Console;
using EventSourcing.Merging;
using System.Runtime.CompilerServices;

var dbConnectionString1 = @"Data Source=c:\\tmp\\mydb.db;";
var dbConnectionString2 = @"Data Source=c:\\tmp\\mydb2.db;";

// uncomment these lines to create one event in each db
//await EventsCreator.CreateEvent(dbConnectionString1);
//await EventsCreator.CreateEvent(dbConnectionString2);

await EventMerger.Merge(dbConnectionString1, dbConnectionString2);
// after the merge you will find a file with events from db1, a file with events from db2,
// and a file that contains the wrong merge: twice the events from db1
Console.ReadKey();




