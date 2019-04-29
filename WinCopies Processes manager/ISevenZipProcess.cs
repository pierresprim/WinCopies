using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopiesProcessesManager
{

    internal interface ISevenZipProcessInternal : ISevenZipProcess

    {

        SevenZip.SevenZipBase InnerProcess { get; }

    }

    public interface ISevenZipProcess
    {

        byte PercentDone { get; }

        byte FilePercentDone { get; }

        string DestPath { get; }

        bool IsBusy { get; }

        bool ExceptionOccurred { get; }

    }
}
