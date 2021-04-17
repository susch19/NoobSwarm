using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NoobSwarm.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int bla;

        public string Greeting => "Welcome to Avalonia!";

        public int Bla
        {
            get => bla;
            set => this.RaiseAndSetIfChanged(ref bla, value);
        }

        public ICommand ClickCommand { get; set; }

        public MainWindowViewModel()
        {
            ClickCommand = ReactiveCommand.Create(() => Bla++);
        }
    }
}
