using System.CommandLine;



var rootCommand = new RootCommand("Root command for file Bundler CLI");
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
var outputOption = new Option<FileInfo>(new[] { "--output", "-o" }, "File path and name");
var languageOption = new Option<string>(new[] { "--language", "-l" }, "An option that must be one of the values of a static list")
    .FromAmong("cs", "sql", "vb", "java", "python", "c", "cpp", "js", "html", "all");
languageOption.IsRequired = true;
string[] lang = new string[] { "cs", "sql", "vb", "java", "py", "c", "cpp", "js", "html" };
var noteOption = new Option<bool>(new[] { "--note", "-n" }, "Copy the name and path of file to the new file");
var sortOption = new Option<string>(new[] { "--sort", "-s" }, "Sort the files").FromAmong("name", "type");
var remove_empty_lines = new Option<bool>(new[] { "--remove", "-r" }, "Remove empty lines");
var authorOption = new Option<string>(new[] { "--author", "-a" }, "Write the author name");

var creteRspCommand = new Command("create-rsp", "Create response file");

rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(creteRspCommand);


bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(remove_empty_lines);
bundleCommand.AddOption(authorOption);

bundleCommand.SetHandler((output, language, note, sort, remove, author) =>
{
    string outputPath = output.FullName;
    DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
    FileInfo[] files = directoryInfo.GetFiles();
    DirectoryInfo[] directories = directoryInfo.GetDirectories("*.", SearchOption.AllDirectories);
    foreach (DirectoryInfo dir in directories)
    {
        if (dir.Name == "bin" || dir.Name == "debag")
        {
            continue;
        }
        FileInfo[] tempFiles = dir.GetFiles();
        FileInfo[] files2 = new FileInfo[files.Length];
        for (int k = 0; k < files.Length; k++)
        {
            files2[k] = files[k];
        }
        files = new FileInfo[tempFiles.Length + files2.Length];
        int i = 0;
        for (; i < files2.Length; i++)
        {
            files[i] = files2[i];
        }
        for (int j = 0; j < tempFiles.Length && i < files.Length; i++, j++)
        {
            files[i] = tempFiles[j];
        }
    }
    if (sort == "type")
    {
        files = files.OrderBy(file => Path.GetExtension(file.Name)).ToArray();
    }
    else
    {
        files = files.OrderBy(file => Path.GetFileNameWithoutExtension(file.Name)).ToArray();
    }

    int count = 0;
    foreach (FileInfo file in files)
    {
        if (language == "all")
        {
            bool flag = false;
            for (int i = 0; i < lang.Length; i++)
            {
                if (file.Extension == $".{lang[i]}")
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(outputPath))
                    {
                        int cnt = 0;
                        string[] fileContent = File.ReadAllLines(file.FullName);
                        if (remove)
                        {
                            fileContent = File.ReadAllLines(file.FullName).Where(line => line != string.Empty).ToArray();
                        }
                        if (author != "" && count < 1)
                        {
                            writer.WriteLine($"Author: {author} \n");
                            count++;
                        }
                        if (note)
                        {
                            writer.WriteLine("//" + file.FullName + "\n" + file.Name + "\n");
                        }
                        for (int i = 0; i < fileContent.Length - cnt; i++)
                        {
                            writer.WriteLine(fileContent[i]);
                        }
                        writer.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to file: {ex.Message}");
                }
            }
        }

        else
        if (file.Extension == $".{language}")
        {
            try
            {
                using (StreamWriter writer = File.AppendText(outputPath))
                {
                    int cnt = 0;
                    string[] fileContent = File.ReadAllLines(file.FullName);
                    if (remove)
                    {
                        fileContent = File.ReadAllLines(file.FullName).Where(line => line != string.Empty).ToArray();
                    }
                    if (author != "" && count < 1)
                    {
                        writer.WriteLine($"Author: {author} \n");
                        count++;
                    }
                    if (note)
                    {
                        writer.WriteLine("//" + file.FullName + "\n" + file.Name + "\n");
                    }
                    for (int i = 0; i < fileContent.Length - cnt; i++)
                    {
                        writer.WriteLine(fileContent[i]);
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
    }

    Console.WriteLine("Operation completed successfully.");
}, outputOption, languageOption, noteOption, sortOption, remove_empty_lines, authorOption);

creteRspCommand.SetHandler(() =>
{
    Console.WriteLine("enter name response file");
    string rspName = Console.ReadLine();
    Console.WriteLine("enter name bundle file");
    string bundleName = Console.ReadLine();
    Console.WriteLine("choose which language to enter to the file");
    string language = Console.ReadLine();
    Console.WriteLine("Do you want to enter the file name?");
    string note = Console.ReadLine();
    Console.WriteLine("Do you want to sort by type?");
    string sort = Console.ReadLine();
    Console.WriteLine("Do you want to remove empty lines?");
    string remove = Console.ReadLine();
    Console.WriteLine("if you want to enter the author name enter his name, if not enter none");
    string author = Console.ReadLine();
    using (StreamWriter writer = File.AppendText(rspName))
    {
        writer.WriteLine($"bundle\n-o\n{bundleName}\n-l\n{language}");
        if (note == "yes")
            writer.WriteLine("-n");
        if (sort == "yes")
            writer.WriteLine("-s\ntype");
        if (remove == "yes")
            writer.WriteLine("-r");
        if (author != "none")
            writer.WriteLine($"-a\n{author}");
    }
});

rootCommand.InvokeAsync(args);





