using System;
using System.Collections.Generic;
using System.IO;
using com.ih.util.custom.errors.Domain;
using Newtonsoft.Json;

namespace com.ih.util.custom.errors.Utils;

public static class LogUtil
{
    private static string LogFileSlug { get; set; }
    private static readonly string LogsPathRoot = AppDomain.CurrentDomain.BaseDirectory + @"Log";
    public static bool LogInitialized = false;

    public static readonly string ContentLineSeparator =
        @"-------------------------------------------------------------------------------------------------------------------------------------------------";

    public static void Initialize(string logFileSlug, Guid logId)
    {
        LogFileSlug = logFileSlug;

        var list = new List<string>();
        
        list.Add(ContentLineSeparator);
        list.AddRange(ContentHeader("Log Initialized"));
        list.Add(ContentLineSeparator);
        list.Add("Log Slug : " + logFileSlug);
        list.Add("Log Path : " + LogsPathRoot);
        list.Add(ContentLineSeparator);
        
        Register(list, logId);

        LogInitialized = true;
    }

    public static List<string> ContentHeader(string title)
    {
        var response = new List<string>();
        var freeSpace = ContentLineSeparator.Length - 6;

        if (title.Length.Equals(freeSpace))
        {
            response.Add("-- " + title + " --");
        }
        else if (title.Length > freeSpace)
        {
            response.Add("-- " + title.Substring(0, freeSpace) + " --");
        }
        else
        {
            var content = "-- " + title + " ";

            for (int i = 0; i < (freeSpace - title.Length); i++)
            {
                content += "-";
            }

            content += "--";

            response.Add(content);
        }

        return response;
    }

    public static List<string> ConsoleLogFormatInternalInformation(List<KeyValuePair<string, string>> values,
        bool isSingle)
    {
        List<string> consoleLog = new List<string>();

        if (values.Count > 0)
        {
            var internalInterval = 0;

            if (!isSingle)
            {
                values.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.Key))
                    {
                        if (x.Key.Length > internalInterval)
                        {
                            internalInterval = x.Key.Length;
                        }
                    }
                });
            }

            values.ForEach(value =>
            {
                if (!string.IsNullOrEmpty(value.Key))
                {
                    string printKey = "|    ";
                    string printValue = string.Empty;

                    var remaining = 85 - value.Key.Length;

                    printKey += "  " + value.Key;

                    if (!isSingle)
                    {
                        var spacesForPoint = string.Empty;
                        var remainingForPoint = (internalInterval - value.Key.Length) + 1;

                        for (int i = 0; i < remainingForPoint; i++)
                        {
                            spacesForPoint += " ";
                        }

                        printKey += spacesForPoint + ": " + value.Value;

                        var spaces = string.Empty;
                        var remainingForFinal = ((85 - 3 - internalInterval) - value.Value.Length) + 2;

                        for (int i = 0; i < remainingForFinal; i++)
                        {
                            spaces += " ";
                        }

                        printKey += spaces + "|";
                    }
                    else
                    {
                        var spaces = string.Empty;

                        for (int i = 0; i < remaining + 2; i++)
                        {
                            spaces += " ";
                        }

                        printKey += spaces + "|";
                    }

                    consoleLog.Add(printKey + printValue);
                }
            });
        }

        return consoleLog;
    }

    public static void RegisterLine(int count = 1)
    {
        var list = new List<string>();

        for (int i = 0; i < count; i++)
        {
            list.Add("\t");
        }
        
        Register(list);
    }
    
    public static void Register(string trace, Guid logId = default, bool printConsole = true)
    {
        Register(new List<string>() { trace }, logId, printConsole);
    }

    public static void Register(CustomErrorException trace, Guid logId = default,  bool print = true)
    {
        var list = new List<string>();

        list.AddRange(ContentHeader("Custom Exception: " + trace.code));
        list.Add("Message             : " + trace.message);
        list.Add("Status              : " + trace.status);
        list.Add("Details             : " + JsonConvert.SerializeObject(trace.details));
        list.Add("ContentBodyRequest  : " + JsonConvert.SerializeObject(trace.contentBodyRequest));
        list.Add("ContentBodyResponse : " + JsonConvert.SerializeObject(trace.contentBodyResponse));
        list.Add(ContentLineSeparator);

        Register(list, logId, print);
    }

    public static void Register(Exception trace, Guid logId = default,  bool print = true)
    {
        var list = new List<string>();

        list.AddRange(ContentHeader("Exception: "));
        list.Add("Message             : " + trace.Message);
        list.Add("StackTrace          : " + trace.StackTrace);
        list.Add("ToString            : " + trace);
        list.Add(ContentLineSeparator);

        Register(list, logId, print);
    }

    public static void Register(List<string> consoleLog, Guid logId = default, bool print = true)
    {
        if (consoleLog?.Count > 0)
        {
            try
            {
                var logFile = LogsPathRoot + @"/" + (!string.IsNullOrEmpty(LogFileSlug) ? LogFileSlug : "IHLog") + "_" +
                              DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                if (!File.Exists(logFile))
                {
                    File.Create(logFile).Dispose();
                }

                if (logId.Equals(default))
                {
                    logId = Guid.NewGuid();
                }

                using (var outputFile = new StreamWriter(logFile, true))
                {
                    consoleLog.ForEach(line =>
                    {
                        var consoleLine = "[ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ] [" + logId + "] " + line;

                        if (print)
                        {
                            if (line.Equals("\t"))
                            {
                                Console.WriteLine("");
                            }
                            else
                            {
                                Console.WriteLine(consoleLine);
                            }
                        }

                        if (line.Equals("\t"))
                        {
                            outputFile.WriteLine(line);
                        }
                        else
                        {
                            outputFile.WriteLine(consoleLine);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ] [" + logId + "] " + ContentLineSeparator);
                Console.WriteLine("[ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ] [" + logId + "] Error on Log Register ");
                Console.WriteLine("[ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ] [" + logId + "] " + ContentLineSeparator);
            }
        }
    }
}