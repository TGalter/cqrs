// Este arquivo é usado para referenciar o assembly da camada de aplicação
// para facilitar a descoberta de tipos pelo MediatR, AutoMapper, etc.

namespace App.Behaviors
{
    public static class AssemblyReference
    {
        public static readonly System.Reflection.Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
