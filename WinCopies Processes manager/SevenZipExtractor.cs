using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZip;
using WinCopies.Util;

namespace WinCopiesProcessesManager
{
    public class SevenZipExtractor : INotifyPropertyChanged, ISevenZipProcessInternal
    {

        /// <summary>
        /// The method that is called to set a value to a property. If succeed, then call the <see cref="OnPropertyChanged(string, object, object)"/> method. See the Remarks section.
        /// </summary>
        /// <param name="propertyName">The name of the property to set in a new value</param>
        /// <param name="fieldName">The name of the field to store the new value. This must the field that is called by the property accessors (get and set).</param>
        /// <param name="newValue">The value to set in the property</param>
        /// <param name="declaringType">The declaring type of both the property and its associated field</param>
        /// <remarks>To use this method, you need to work with the WinCopies Framework Property changed notification pattern. See the website of the WinCopies Framework for more details.</remarks>
        protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType = null)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) OnPropertyChanged(propertyName, oldValue, newValue);

        }

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

        private SevenZip.SevenZipExtractor InnerProcess { get; } = null;

        SevenZipBase ISevenZipProcessInternal.InnerProcess => InnerProcess;

        private readonly byte _percentDone = 0;

        public byte PercentDone { get => _percentDone; private set => OnPropertyChanged(nameof(PercentDone), nameof(_percentDone), value, typeof(SevenZipExtractor)); }

        private readonly ArchiveFileInfo? _fileInfo = null;

        public ArchiveFileInfo? FileInfo { get => _fileInfo; private set => OnPropertyChanged(nameof(FileInfo), nameof(_fileInfo), value, typeof(SevenZipExtractor)); }

        private readonly byte _filePercentDone = 0;

        public byte FilePercentDone { get => _filePercentDone; private set => OnPropertyChanged(nameof(FilePercentDone), nameof(_filePercentDone), value, typeof(SevenZipExtractor)); }

        //private readonly string _sourcePath = null;

        //public string SourcePath { get => _sourcePath; private set => OnPropertyChanged(nameof(SourcePath), nameof(_sourcePath), value, typeof(SevenZipExtractor)); }

        private readonly string _destPath = null;

        public string DestPath { get => _destPath; private set => OnPropertyChanged(nameof(DestPath), nameof(_destPath), value, typeof(SevenZipExtractor)); }

        private readonly bool _isBusy = false;

        public bool IsBusy { get => _isBusy; private set => OnPropertyChanged(nameof(IsBusy), nameof(_isBusy), value, typeof(SevenZipExtractor)); }

        public bool ExceptionOccurred { get; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public SevenZipExtractor(SevenZip.SevenZipExtractor extractor)
        {

            InnerProcess = extractor;

            extractor.Extracting += Extractor_Extracting;

            extractor.FileExtractionStarted += Extractor_FileExtractionStarted;

            // extractor.FileExtractionFinished += Extractor_FileExtractionFinished;

            extractor.ExtractionFinished += Extractor_ExtractionFinished;

            extractor.FileExists += Extractor_FileExists;

        }

        public void BeginExtractArchive(string directory)

        {

            DestPath = directory;

            IsBusy = true;

            InnerProcess.BeginExtractArchive(directory);

        }

        public void BeginExtractFile(int index, Stream stream)

        {

            if (stream is FileStream fs)

                DestPath = fs.Name;

            IsBusy = true;

            InnerProcess.BeginExtractFile(index, stream);

        }

        public void BeginExtractFile(string fileName, Stream stream)

        {

            if (stream is FileStream fs)

                DestPath = fs.Name;

            IsBusy = true;

            InnerProcess.BeginExtractFile(fileName, stream);

        }

        public void BeginExtractFiles(string directory, params int[] indexes)

        {

            DestPath = directory;

            IsBusy = true;

            InnerProcess.BeginExtractFiles(directory, indexes);

        }

        public void BeginExtractFiles(string directory, params string[] fileNames)

        {

            DestPath = directory;

            IsBusy = true;

            InnerProcess.BeginExtractFiles(directory, fileNames);

        }

        private void Extractor_FileExists(object sender, FileOverwriteEventArgs e) => throw new NotImplementedException();
        private void Extractor_ExtractionFinished(object sender, EventArgs e) => IsBusy = false;
        private void Extractor_FileExtractionStarted(object sender, FileInfoEventArgs e)

        {

            FileInfo = e.FileInfo;

            FilePercentDone = e.PercentDone;

        }

        private void Extractor_Extracting(object sender, ProgressEventArgs e) => PercentDone = e.PercentDone;
    }
}
