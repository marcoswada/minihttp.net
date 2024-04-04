module program
    sub main()
        'envvars
        'console.writeline(Environment.userprofile)
        console.writeline( Dir ( Environment. ) )
        dim dt() as byte = My.Computer.FileSystem.ReadAllBytes("index.html")
        for i = lbound(dt) to ubound(dt)
            'console.write(chr(dt(i)))
        next
    end sub

    sub envvars()
        'Dim str As [String]
        Dim nl As [String] = Environment.NewLine
        '
        Console.WriteLine()
        Console.WriteLine("-- Environment members --")
        
        '  Invoke this sample with an arbitrary set of command line arguments.
        Console.WriteLine("CommandLine: {0}", Environment.CommandLine)
        
        Dim arguments As [String]() = Environment.GetCommandLineArgs()
        Console.WriteLine("GetCommandLineArgs: {0}", [String].Join(", ", arguments))
        
        '  <-- Keep this information secure! -->
        Console.WriteLine("CurrentDirectory: {0}", Environment.CurrentDirectory)
        
        Console.WriteLine("ExitCode: {0}", Environment.ExitCode)
        
        Console.WriteLine("HasShutdownStarted: {0}", Environment.HasShutdownStarted)
        
        '  <-- Keep this information secure! -->
        Console.WriteLine("MachineName: {0}", Environment.MachineName)
        
        Console.WriteLine("NewLine: {0}  first line{0}  second line{0}" & _
                        "  third line", Environment.NewLine)
        
        Console.WriteLine("OSVersion: {0}", Environment.OSVersion.ToString())
        
        Console.WriteLine("StackTrace: '{0}'", Environment.StackTrace)
        
        '  <-- Keep this information secure! -->
        Console.WriteLine("SystemDirectory: {0}", Environment.SystemDirectory)
        
        Console.WriteLine("TickCount: {0}", Environment.TickCount)
        
        '  <-- Keep this information secure! -->
        Console.WriteLine("UserDomainName: {0}", Environment.UserDomainName)
        
        Console.WriteLine("UserInteractive: {0}", Environment.UserInteractive)
        
        '  <-- Keep this information secure! -->
        Console.WriteLine("UserName: {0}", Environment.UserName)
        
        Console.WriteLine("Version: {0}", Environment.Version.ToString())
        
        Console.WriteLine("WorkingSet: {0}", Environment.WorkingSet)
    end sub
end module