﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using WinCopies.Util;

namespace WinCopiesProcessesManager
{
    public class SevenZipCompressor : INotifyPropertyChanged, ISevenZipProcessInternal
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

        private SevenZip.SevenZipCompressor InnerProcess { get; } = null;

        SevenZip.SevenZipBase ISevenZipProcessInternal.InnerProcess => InnerProcess;

        private readonly bool _filesLoaded = false;

        public bool FilesLoaded { get => _filesLoaded; private set => OnPropertyChanged(nameof(FilesLoaded), nameof(_filesLoaded), value, typeof(SevenZipCompressor)); }

        private readonly int _filesFound = 0;

        public int FilesFound { get => _filesFound; private set => OnPropertyChanged(nameof(FilesFound), nameof(_filesFound), value, typeof(SevenZipCompressor)); }

        private readonly byte _percentDone = 0;

        public byte PercentDone { get => _percentDone; private set => OnPropertyChanged(nameof(PercentDone), nameof(_percentDone), value, typeof(SevenZipCompressor)); }

        //private byte _percentDelta = 0;

        //public byte PercentDelta { get => _percentDelta; private set => OnPropertyChanged(nameof(PercentDelta), nameof(_percentDelta), value, typeof(SevenZipCompressor)); }

        private readonly string _fileName = null;

        public string FileName { get => _fileName; private set => OnPropertyChanged(nameof(FileName), nameof(_fileName), value, typeof(SevenZipCompressor)); }

        private readonly byte _filePercentDone = 0;

        public byte FilePercentDone { get => _filePercentDone; private set => OnPropertyChanged(nameof(FilePercentDone), nameof(_filePercentDone), value, typeof(SevenZipCompressor)); }

        private readonly string _sourcePath = null;

        // todo: really needed?

        public string SourcePath { get => _sourcePath; private set => OnPropertyChanged(nameof(SourcePath), nameof(_sourcePath), value, typeof(SevenZipCompressor)); }

        private string[] _sourcePaths = null;

        public string[] SourcePaths { get => _sourcePaths; set { if (_isBusy) throw new InvalidOperationException("The process is busy."); _sourcePaths = value; } }

        private readonly string _destPath = null;

        public string DestPath { get => _destPath; set { if (_isBusy) throw new InvalidOperationException("The process is busy."); OnPropertyChanged(nameof(DestPath), nameof(_destPath), value, typeof(SevenZipCompressor)); } }

        private readonly bool _isBusy = false;

        public bool IsBusy { get => _isBusy; private set => OnPropertyChanged(nameof(IsBusy), nameof(_isBusy), value, typeof(SevenZipCompressor)); }

        private bool _cancellationPending = false;

        public bool CancellationPending { get => _cancellationPending; set { if (!_isBusy) throw new InvalidOperationException("The process is not busy."); _cancellationPending = value; } }

        public bool ExceptionOccurred { get; internal set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public SevenZipCompressor(SevenZip.SevenZipCompressor compressor)

        {

            InnerProcess = compressor;

            compressor.FilesFound += Compressor_FilesFound;

            compressor.Compressing += Compressor_Compressing;

            compressor.FileCompressionStarted += Compressor_FileCompressionStarted;

            compressor.CompressionFinished += Compressor_CompressionFinished;

        }

        private void Compressor_CompressionFinished(object sender, EventArgs e) => IsBusy = false;

        private void OnCompressionStarting(string archiveName)

        {

            DestPath = archiveName;

            IsBusy = true;

        }

        public void BeginCompressDirectory(string directory)

        {

            SourcePath = directory;

            OnCompressionStarting(_destPath);

            InnerProcess.BeginCompressDirectory(directory, _destPath);

        }

        public void BeginCompressFiles(string[] files)

        {

            SourcePath = System.IO.Path.GetDirectoryName(files[0]);

            OnCompressionStarting(_destPath);

            InnerProcess.BeginCompressFiles(_destPath, files);

        }

        private void Compressor_FileCompressionStarted(object sender, SevenZip.FileNameEventArgs e)
        {
            e.Cancel = CancellationPending;
            FileName = e.FileName;

            FilePercentDone = e.PercentDone;

        }

        private void Compressor_FilesFound(object sender, SevenZip.IntEventArgs e)
        {

            FilesLoaded = true;

            FilesFound = e.Value;

        }

        private void Compressor_Compressing(object sender, SevenZip.ProgressEventArgs e)
        {
            PercentDone = e.PercentDone;

            Debug.WriteLine("PercentDone: " + e.PercentDone.ToString());

            Debug.WriteLine("PercentDelta: " + e.PercentDelta.ToString());
        }
    }
}