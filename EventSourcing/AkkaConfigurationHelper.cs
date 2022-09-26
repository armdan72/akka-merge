using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Akka.Streams.FlowMonitor;

namespace EventSourcing
{
    public static class AkkaConfigurationHelper
    {
        public static Config CreateSqlLiteConfig(
            string dbConnectionString,
            bool readonlyConnection)
        {
            string PersistenceString(
                int x
            )
            {
                return @"
                persistence 
                {
                    journal 
                    {
                        plugin = ""akka.persistence.journal.sqlite""

                        sqlite
                        {
                            class = ""Akka.Persistence.Sqlite.Journal.SqliteJournal, Akka.Persistence.Sqlite""
                            connection-string = """ + dbConnectionString + "\"\n" + @"
                            #auto-initialize = " + (readonlyConnection ? "false" : "true") + @"
                            auto-initialize = true
                        }
                    }
                    snapshot-store
                    {
                        plugin = ""akka.persistence.snapshot-store.sqlite""
                        sqlite
                        {
                            connection-string = """ + dbConnectionString + "\"\n" + @"
                            auto-initialize = true
                        }
                    }
                }";
            }

            return CreateAkkaConfig(445566, PersistenceString);

        }

        public static Config CreatePgsqlConfig(
            string dbConnectionString,
            bool readonlyConnection
        )
        {
            string PersistenceString(
                int x
            )
            {
                return @"
                persistence 
                {
                    journal 
                    {
                        plugin = ""akka.persistence.journal.postgresql""

                        postgresql 
                        {
                            # qualified type name of the PostgreSql persistence journal actor
                            class = ""Akka.Persistence.PostgreSql.Journal.PostgreDbBatchSqlJournal, Akka.Persistence.PostgreSql""

                            # dispatcher used to drive journal actor
                            plugin-dispatcher = ""akka.actor.default-dispatcher""

                            # connection string used for database access
                            connection-string = """ + dbConnectionString + "\"\n" + @"
                            # default SQL commands timeout
                            connection-timeout = 30s

                            # PostgreSql schema name to table corresponding with persistent journal
                            schema-name = public

                            # PostgreSql table corresponding with persistent journal
                            table-name = akka_persisted_events

                            # should corresponding journal table be initialized automatically
                            auto-initialize = " + (readonlyConnection ? "false" : "true") + @"

                            # timestamp provider used for generation of journal entries timestamps
                            timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                        
                            # metadata table
                            metadata-table-name = metadata

                            # defines column db type used to store payload. Available option: BYTEA (default), JSON, JSONB
                            stored-as = BYTEA
                            # serializer = tnw

                            recovery-event-timeout = 4800s

                            circuit-breaker {
                                max-failures = 10
                                call-timeout = 300s
                                reset-timeout = 30s
                            }
                        }
                    }
                    query.journal.sql 
                    {
                         max-buffer-size=10000
                         refresh-interval=1s
                    }
                }";
            }

            return CreateAkkaConfig(445566, PersistenceString);
        }

        //public static Config CreateLinq2DbConfig(
        //    string dbConnectionString
        //)
        //{
        //    string PersistenceString(
        //        int x
        //    ) =>
        //        @"
        //        persistence 
        //        {
        //            publish-plugin-commands = on
        //            journal 
        //            {
        //                plugin = ""akka.persistence.journal.linq2db""

        //                linq2db 
        //                {
        //                    # qualified type name of the PostgreSql persistence journal actor
        //                    class = ""Akka.Persistence.Sql.Linq2Db.Journal.Linq2DbWriteJournal, Akka.Persistence.Sql.Linq2Db""

        //                    # dispatcher used to drive journal actor
        //                    plugin-dispatcher = ""akka.persistence.dispatchers.default-plugin-dispatcher""

        //                    # connection string used for database access
        //                    connection-string = """ + dbConnectionString + "\"\n" + @"
        //                    # Provider name is required.
        //                    # Refer to LinqToDb.ProviderName for values
        //                    # Always use a specific version if possible
        //                    # To avoid provider detection performance penalty
        //                    # Don't worry if your DB is newer than what is listed;
        //                    # Just pick the newest one (if yours is still newer)
        //                    provider-name = ""PostgreSQL95""
        //                    # default SQL commands timeout
        //                    connection-timeout = 30s

        //                    # PostgreSql schema name to table corresponding with persistent journal
        //                    schema-name = public

        //                    # PostgreSql table corresponding with persistent journal
        //                    table-name = persisted_events
        //                    table-compatibility-mode = postgres
        //                    # should corresponding journal table be initialized automatically
        //                    auto-initialize = on

        //                    # timestamp provider used for generation of journal entries timestamps
        //                    timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                        
        //                    # metadata table
        //                    metadata-table-name = metadata

        //                    # defines column db type used to store payload. Available option: BYTEA (default), JSON, JSONB
        //                    stored-as = BYTEA
        //                    serializer = tnw

        //                    recovery-event-timeout = 4800s
        //                    tables.journal {

        //                        #if delete-compatibility-mode is true, both tables are created
        //                        #if delete-compatibility-mode is false, only journal table will be created.
        //                        auto-init = true
        //                    }

        //                    circuit-breaker {
        //                        max-failures = 10
        //                        call-timeout = 300s
        //                        reset-timeout = 30s
        //                    }
        //                }
        //            }
        //            query.journal.sql 
        //            {
        //                 max-buffer-size = 1000
        //                 refresh-interval=1s
        //            }
        //        }";

        //    return CreateAkkaConfig(445566, PersistenceString);
        //}

        private static Config CreateAkkaConfig(
            int port,
            Func<int, string> persistentStringCreator
        )
        {
            //var config = ConfigurationFactory.ParseString(
            //    @"
            //akka {
            //    loglevel = DEBUG
            //    loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
            //    actor {
            //        debug {
            //            receive = on
            //            autoreceive = on
            //            lifecycle = on
            //            event-stream = on
            //            unhandled = on
            //        }
            //        serializers {
            //            tnw = ""TNW.Actors.Serialization.Serializer, TNW.Actors.Serialization""
            //        }
            //        serialization-bindings {
            //            ""System.Object"" = tnw
            //        }
            //        serialization-settings {
            //             tnw {
            //                json-surrogates = [
            //                    ""TNW.Domain.OpenEHR.ConceptModel.ConsultationObservation, TNW.Domain.OpenEHR""
            //                ]
            //            }
            //        }
            //        serialization-identifiers {
            //            # Only necessary as a work-around for a current bug in Akka.
            //            # Once this bug has been fixed. This section can be removed.
            //            ""TNW.Actors.Serialization.Serializer, TNW.Actors.Serialization"" = 100
            //        }
            //    }
                
            //    " + persistentStringCreator(port)
            //          + "}"
            //);
            var config = ConfigurationFactory.ParseString(
                @"
            akka {
                stdout-loglevel = DEBUG
                loglevel = DEBUG
                log-config-on-start = on
                actor {
                    debug {
                        receive = on
                        autoreceive = on
                        lifecycle = on
                        event-stream = on
                        unhandled = on
                    }
                
                }
                
                " + persistentStringCreator(port)
                      + "}"
            );
            return config;
        }
    }
}
