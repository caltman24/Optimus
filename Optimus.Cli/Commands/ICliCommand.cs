using System.CommandLine;
using System.CommandLine.Invocation;

namespace Optimus.Cli;

public interface ICliCommand
{
    public static abstract RootCommand BuildCommand();
}