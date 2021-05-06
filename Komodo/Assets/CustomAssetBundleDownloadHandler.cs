using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;


using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

public class CustomAssetBundleDownloadHandler : DownloadHandlerScript
{

    public int contentLength { get { return _received > _contentLength ? _received : _contentLength; } }

    private int _contentLength;
    private int _received;
    private FileStream _stream;

    private SafeFileHandle handle;
    public byte[] savedByes;
    public CustomAssetBundleDownloadHandler(string localFilePath, int bufferSize = 4096, FileShare fileShare = FileShare.ReadWrite) : base(new byte[bufferSize])
    {
        string directory = Path.GetDirectoryName(localFilePath);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        //   if (!Directory.Exists(localFilePath))
        //     Directory.CreateDirectory(localFilePath);

        _contentLength = -1;
        _received = 0;

        //gets filled up automatically and writen,      can we modify this?
        savedByes = new byte[bufferSize];


   //     handle = 
      //  _stream = new FileStream(localFilePath, FileMode.OpenOrCreate, FileAccess.Write);
  //     _stream.
      //  new SafeFileHandle(IntPtr.//, fileShare, bufferSize);
        //  _stream = new FileStream(localFilePath, FileMode.OpenOrCreate, FileAccess.Write, fileShare, bufferSize);
        //_stream = new FileStream(localFilePath, FileMode.OpenOrCreate, FileAccess.Write, fileShare, bufferSize);
    }

    protected override float GetProgress()
    {
        return contentLength <= 0 ? 0 : Mathf.Clamp01((float)_received / (float)contentLength);
    }

    protected override void ReceiveContentLength(int contentLength)
    {
        _contentLength = contentLength;
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || data.Length == 0) return false;

     //   Array.Copy(data, 0, savedByes, _received, dataLength);
        //savedByes[_received] = 

        _received += dataLength;
 
     //   _stream.Write(data, 0, dataLength);

        return true;
    }

    protected override void CompleteContent()
    {
        CloseStream();
    }

    public new void Dispose()
    {
        CloseStream();
        base.Dispose();
    }

    private void CloseStream()
    {
        if (_stream != null)
        {
            _stream.Dispose();
            _stream = null;
        }
    }

}
//using System;
//using System.IO;
//using UnityEngine;
//using UnityEngine.Networking;

//class CustomAssetBundleDownloadHandler : DownloadHandlerScript
//{
//    private int expected = -1;
//    private int received = 0;
//    private string filepath;
//    private FileStream fileStream;
//    private bool canceled = false;

//    public CustomAssetBundleDownloadHandler(byte[] buffer, string filepath)
//      : base(buffer)
//    {
//        if(buffer == null || buffer.Length < 1)
//        {
//            Debug.LogError("Received Asset with 0 byres at CustomAsetBundleDownloader.cs");
//                return;
//        }

//        this.filepath = filepath;
//      //  fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length);
//        fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
//    }

//    protected override byte[] GetData() { return null; }

//    protected override bool ReceiveData(byte[] data, int dataLength)
//    {
//        if (data == null || data.Length < 1)
//        {
//            return false;
//        }
//        received += dataLength;

//        if (!canceled) {

//            //int byteOffset = 0;
//            //for

//            fileStream.Write(data, 0, dataLength);

//        };
//        return true;
//    }

//    protected override float GetProgress()
//    {
//        if (expected < 0) return 0;
//        return (float)received / expected;
//    }
//    public byte[] savedBytes;
//    protected override void CompleteContent()
//    {
//       // savedBytes = new byte[received];
//       //Buffer.BlockCopy()
//        fileStream.Close();
//    }

//    protected override void ReceiveContentLength(int contentLength)
//    {
//        Debug.LogError("RECEIVE CONTENT LENGHT :" + contentLength);
//        expected = contentLength;
//    }

//    public void Cancel()
//    {
//        canceled = true;
//        fileStream.Close();
//        File.Delete(filepath);
//    }
//    //private string _targetFilePath;
//    //private Stream _fileStream;


//    //public CustomAssetBundleDownloadHandler(string targetFilePath)
//    //    : base(new byte[4096]) // use pre-allocated buffer for better performance
//    //{
//    //    _targetFilePath = targetFilePath;
//    //}

//    //byte[] dataOut = new byte[1000];
//    //protected override bool ReceiveData(byte[] data, int dataLength)
//    //{


//    //    // create or open target file
//    //    if (_fileStream == null)
//    //    {

//    //          _fileStream = File.OpenWrite(_targetFilePath);
//    //      //    _fileStream.WriteTimeout = 1000;
//    //        //  File.WriteAllBytes(filepath, www.bytes);
//    //    }
//    //    //using(Stream s = new Stream(_fileStream))
//    //    //{


//    //    //}
//    //    //using (StreamWriter sw = new StreamWriter(_fileStream))
//    //    //{
//    //    //    //int c = 1000;
//    //    //    //int times = 0;
//    //    //    //Console.Write(sw.NewLine.Length);
//    //    //    //while (sw.NewLine.Length >= 0)
//    //    //    //{


//    //    //    //}
//    //    //    //while (sr.Peek() >= 0)
//    //    //    //{
//    //    //    //_fileStream.WriteTimeout = 1000;
//    //    //    //_fileStream.Write(data, c, c);
//    //    //    //times++;
//    //    //    //   c = new char[5];
//    //    //    //   sr.ReadBlock(c, 0, c.Length);
//    //    //    //      Console.WriteLine(c);
//    //    //    //    }
//    //    //}

//    //    //using (StreamReader sr = new StreamReader(_fileStream))
//    //    //{
//    //    //    char[] c = null;
//    //    //    //while (sr.Peek() >= 0)
//    //    //    //{
//    //    //    //    c = new char[5];
//    //    //    // //   sr.ReadBlock(c, 0, c.Length);
//    //    //    //    Console.Write(c);
//    //    //    //}
//    //    //}


//    //   _fileStream.Write(data, 0, dataLength);

//    //    return true;
//    //}

//    ////protected override byte[] GetData()
//    ////{
//    ////    using (StreamReader sr = new StreamReader(_fileStream))
//    ////    {
//    ////        char[] c = null;
//    ////        while (sr.Peek() >= 0)
//    ////        {
//    ////            c = new char[5];
//    ////            sr.ReadBlock(c, 0, c.Length);
//    ////            Console.WriteLine(c);
//    ////        }
//    ////    }


//    ////    return base.GetData();
//    ////}
//    ////protected override void ReceiveContentLength(int contentLength)
//    ////{
//    ////    base.ReceiveContentLength(contentLength);
//    ////}
//    //protected override void CompleteContent()
//    //{
//    //    // close and save
//    //    _fileStream.Close();
//    //}
//}