

//#define TRILIB_USE_ZIP
#pragma warning disable 649


using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicerSamples;
using BzKovSoft.ObjectSlicer.EventHandlers;
using System.Threading.Tasks;




using TiltBrushToolkit;
using UnityEngine.UI;
using UnityEditor;
#if TRILIB_USE_ZIP
#if !UNITY_EDITOR && (NETFX_CORE || NET_4_6 || NET_STANDARD_2_0) && !ENABLE_IL2CPP && !ENABLE_MONO
using System.IO.Compression;
#else
using ICSharpCode.SharpZipLib.Zip;
#endif
#endif
namespace TriLib
{
    namespace Samples
    {
        public class DownloadSample : MonoBehaviour
        {
            
            public Text downloadText;
            //url asset list
            public String_List url_List;

            //example urls in case the asset list gets cleared.
            //    "https://cdn.jsdelivr.net/gh/leation/mydata/Bee.glb",
            //    "http://ricardoreis.net/trilib/test2.zip",
            //    "http://ricardoreis.net/trilib/test3.zip",
            //    "http://ricardoreis.net/trilib/test1.3ds"

            //Stores a reference for file downloaders
            private UnityWebRequest[] _fileDownloaders;

            //Reference for the latest loaded GameObject
            private GameObject _loadedGameObject;

            //material to show globally for slice
            public Material defaultMaterialForSlice;

            //root object of url assets
            GameObject parentOfLoadedObj;

            //loaded_url_list
            public List<GameObject> goList = new List<GameObject>();

            //private void Awake()
            //{
            //    PlayerSettings.SetPropertyString("emscriptenArgs", "-s MEMFS_APPEND_TO_TYPED_ARRAYS=1", BuildTargetGroup.WebGL);
            //}
            private IEnumerator Start()
            {
              
                //For each asset on the list, we create a slot for a new WWW instance
                _fileDownloaders = new UnityWebRequest[url_List.url_list.Count];

                //Do not continue until all objects are finished loading
                yield return StartCoroutine(LoadAllURL_GO());

                //change flag load state
                ClientSpawnManager.Instance.isURL_Loading_Finished = true;
            }


            public IEnumerator LoadAllURL_GO()
            {
                parentOfLoadedObj = new GameObject("Loaded_Object_List");

                //wait for each loaded object to process
                for (int i = 0; i < url_List.url_list.Count; i++)
                    yield return StartCoroutine(LoadNetworkObjects(i));

                //HavetoIterateAndSetUp_ChildrenOfURLObjects here to maintain index order for ui setup
                for (int i = 0; i < goList.Count; i++)
                {

                    //only set up children if whole objectflag is off
                    if (!url_List.url_list[i].isWholeObject)
                    {
                        //activate to allow GO setup to happen
                        goList[i].transform.parent.gameObject.SetActive(true);

                        SetUp_RecursiveChild_RBAndColliders(goList[i].transform, i);

                        //de-activate root imported asset once their individual setup is complete

                    }
                    goList[i].transform.parent.gameObject.SetActive(false);
                }

            }
            List<Bounds> subObjectBounds = new List<Bounds>();

            [Tooltip("To avoid importing very large objects into the scene")]
            [SerializeField] private float _defaultSizeToLoadGO = 2;

            public IEnumerator LoadNetworkObjects(int index)
            {
                GameObject loadedGO = null;
                Transform newGOParent = new GameObject(index.ToString()).transform;//SetParent( parentOfLoadedObj.transform);

                //clear subobjectlist for new object processiong
                subObjectBounds.Clear();

                //what if loadedgo is the wrong pivot - wrong bounds
                Bounds bounds = new Bounds();// subObjectBounds[0].center, Vector3.one * 0.02f);

                //obtain and wait for file
                yield return StartCoroutine(LoadFileFrom_URL(index, url_List.url_list[index].url, value =>
                {

                    loadedGO = value;

                    //reset rotation to avoid bounding detecting errors
                    loadedGO.transform.rotation = Quaternion.identity;

                    //obtain all bounds from skinned mesh renderer and skinned mesh remderer
                    CombineMesh(loadedGO.transform, subObjectBounds);

                    bounds = new Bounds(subObjectBounds[0].center, Vector3.one * 0.02f);

                    //set bounds from all subobjects
                    for (int i = 0; i < subObjectBounds.Count; i++)
                        bounds.Encapsulate(new Bounds(subObjectBounds[i].center, subObjectBounds[i].size));


                }));


                ////Set up physics properties
                //Rigidbody tempRB = newGOParent.gameObject.AddComponent<Rigidbody>();
                //tempRB.isKinematic = true;
                //tempRB.useGravity = false;

                //set collider properties
                var wholeCollider = newGOParent.gameObject.AddComponent<BoxCollider>();
                wholeCollider.center = bounds.center;//  - ;//loadedGO.transform.InverseTransformPoint(bounds.center);// - Vector3.zero;// aggregatedMeshFilterList[0].center;
                wholeCollider.size = bounds.size;

                //set up reference to use with network
                ClientSpawnManager.Instance.LinkNewNetworkObject(newGOParent.gameObject, index, url_List.url_list[index].id);
                newGOParent.gameObject.GetComponent<Net_Register_GameObject>().isAutomatic_Register = true;

                newGOParent.gameObject.gameObject.tag = "Interactable";

                //ObjectSlicerSample oSS = loadedGO.AddComponent<ObjectSlicerSample>();
                //oSS.defaultSliceMaterial = defaultMaterialForSlice;
                //loadedGO.AddComponent<KnifeSliceableAsync>();
                //loadedGO.AddComponent<BzSmoothDepenetration>();

                //set animation to be affected by physics
                var Anim = loadedGO.GetComponent<Animation>();

                if (Anim)
                    loadedGO.GetComponent<Animation>().animatePhysics = true;

                newGOParent.gameObject.SetActive(false);

                //set correct pivot for correct deformation (scale grab)
                newGOParent.transform.position = bounds.center;

                loadedGO.transform.SetParent(newGOParent.transform, true);

                wholeCollider.center += loadedGO.transform.localPosition;

                newGOParent.transform.position = Vector3.zero;

                //CHECK IF OBJECT IS TO BIG TO DOWNSCALE TO DEFAULT
                while (bounds.extents.x > _defaultSizeToLoadGO || bounds.extents.y > _defaultSizeToLoadGO || bounds.extents.z > _defaultSizeToLoadGO || bounds.extents.x < -_defaultSizeToLoadGO || bounds.extents.y < -_defaultSizeToLoadGO || bounds.extents.z < -_defaultSizeToLoadGO)
                {
                    newGOParent.transform.localScale *= 0.9f;
                    bounds.extents *= 0.9f;
                }

                //turn off whole colliders for those set to be deconstructive
                if (!url_List.url_list[index].isWholeObject)
                {
                    wholeCollider.enabled = false;
                    CheckForIndividualFilterAndSkinn(loadedGO, index, setNetwork: false);
                }

                //set custom properties
                newGOParent.position += url_List.url_list[index].position;
                newGOParent.transform.localScale *= url_List.url_list[index].scale;//new Vector3(url_List.url_list[index].scale, url_List.url_list[index].scale, url_List.url_list[index].scale); 
                newGOParent.rotation = Quaternion.Euler(url_List.url_list[index].euler_rotation);


                newGOParent.SetParent(parentOfLoadedObj.transform, true);

                goList.Add(loadedGO);

            }

            void SetUp_RecursiveChild_RBAndColliders(Transform trans, int uiIndex)
            {
                //search each child object for components
                foreach (Transform child in trans)
                {
                    CheckForIndividualFilterAndSkinn(child.gameObject, uiIndex);

                    //deeper children search
                    if (child.childCount > 0)
                        SetUp_RecursiveChild_RBAndColliders(child, uiIndex);
                }
            }


            public void CombineMesh(Transform trans, List<Bounds> combinedMeshList)
            {
                //search each child object for components
                foreach (Transform child in trans)
                {
                    Bounds mf = Obtain_GO_MeshFilter(child.gameObject);
                    if (mf != null)
                        combinedMeshList.Add(mf);

                    //deeper children search
                    if (child.childCount > 0)
                        CombineMesh(child, combinedMeshList);
                }
            }




            public void CheckForIndividualFilterAndSkinn(GameObject gameObjectToCheck, int indexForUISelection = -1, bool setNetwork = true)
            {

                bool hasMeshFilter = false;
                MeshFilter meshF = default;

                bool hasSkinnedMR = false;
                SkinnedMeshRenderer skinned = default;

                //Access what components are available
                try
                {
                    meshF = gameObjectToCheck.GetComponent<MeshFilter>();
                    if (meshF != null)
                        hasMeshFilter = true;
                }
                catch
                { }
                try
                {
                    skinned = gameObjectToCheck.GetComponent<SkinnedMeshRenderer>();
                    if (skinned != null)
                        hasSkinnedMR = true;
                }
                catch { }

                try
                {

                    if (hasMeshFilter || hasSkinnedMR)
                    {
                        //set grab tag
                        gameObjectToCheck.gameObject.tag = "Interactable";

                        //   MeshCollider tempCollider = default;
                        BoxCollider tempCollider = default;

                        if (hasMeshFilter)
                        {
                            tempCollider = gameObjectToCheck.gameObject.AddComponent<BoxCollider>();
                            // tempCollider = gameObjectToCheck.gameObject.AddComponent<MeshCollider>();
                            //tempCollider.convex = true;
                            //tempCollider.sharedMesh = meshF.sharedMesh;
                        }
                        else if (hasSkinnedMR)
                        {
                            if (skinned.rootBone)
                            {
                                tempCollider = gameObjectToCheck.gameObject.AddComponent<BoxCollider>();
                                //gameObjectToCheck = skinned.rootBone.gameObject;

                                //tempCollider = gameObjectToCheck.gameObject.AddComponent<MeshCollider>();
                                //tempCollider.convex = true;
                                //tempCollider.sharedMesh = skinned.sharedMesh;
                            }
                        }
                        //CHECK TO SEE IF OBJECT DOES HAVE A RIGID BODY 
                        //Rigidbody tempRB = gameObjectToCheck.gameObject.AddComponent<Rigidbody>();

                        ////Turn off physic properties
                        //tempRB.isKinematic = true;
                        //tempRB.useGravity = false;
                        //   tempRB.velocity = Vector3.zero;

                        //IF NOT REGISTERED INITIATY(PARENT OBJECTS FOR UI LIST -THEN REGISTER THEM NOW CHILDREN OF URLOBJECTS NO NEED FOR URL INDIXES NOW DO URL THEN DO CHILD THEN DO SCENE REGISTER URL OBJECTS->CHILD OBJECTS OF URL->SCENE OBJECTS
                        if (setNetwork)
                        {
                            ClientSpawnManager.Instance.LinkNewNetworkObject(gameObjectToCheck, indexForUISelection);
                            //ClientSpawnManager.Instance.LinkNewNetworkObject(gameObjectToCheck, ClientSpawnManager.Instance._EntityID_To_NetObject.Count);
                            gameObjectToCheck.GetComponent<Net_Register_GameObject>().isAutomatic_Register = true;
                        }

                        // SLICING ITEMS
                        //ObjectSlicerSample oSS = gameObjectToCheck.AddComponent<ObjectSlicerSample>();
                        //oSS.defaultSliceMaterial = defaultMaterialForSlice;
                        //gameObjectToCheck.AddComponent<KnifeSliceableAsync>();
                        //gameObjectToCheck.AddComponent<BzSmoothDepenetration>();
                        // }

                        var Anim = gameObjectToCheck.GetComponent<Animation>();

                        if (Anim)
                            gameObjectToCheck.GetComponent<Animation>().animatePhysics = true;


                    //    gameObjectToCheck.gameObject.tag = "Interactable";
                    }
                }
                catch { }
            }



            public Bounds Obtain_GO_MeshFilter(GameObject gameObjectToCheck)
            {
                //  Mesh mesh = default;
                MeshRenderer mf = null;
                SkinnedMeshRenderer smr = null;

                mf = gameObjectToCheck.GetComponent<MeshRenderer>();//t<MeshFilter>();
                smr = gameObjectToCheck.GetComponent<SkinnedMeshRenderer>();

                if (mf != null)
                    return mf.bounds;//sharedMesh;
                else
                {
                    if (smr != null)
                        return smr.bounds;
                    else
                        return default;
                }

            }


            public IEnumerator LoadFileFrom_URL(int index, string url, System.Action<GameObject> result)
            {

              //  yield return new WaitUntil(() => !isUnloading);

                //Gets current WWW instance (if avaliable)
                var fileDownloader = _fileDownloaders[index];

                //    Debug.Log("URL LOAD");
                //Checks if current file downloader exists
                if (fileDownloader == null)
                {
                    //When clicking the "Load" button

                    //Gets current file name, extension, local path and local filename
                    var fileName = FileUtils.GetFilenameWithoutExtension(url);//FileUtils.GetShortFilename(url_List.url_list[index].name); // FileUtils.GetFilenameWithoutExtension(url);
                                                                              //    var objectName = 
                    var fileExtension = FileUtils.GetFileExtension(url);

                    var objName =UnityWebRequest.EscapeURL(url_List.url_list[index].name); 

                    //to make objecs unique beased on their name
                    var localFilePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName + objName);//fileName);
                    
                    var localFilename = string.Format("{0}/{1}{2}", localFilePath, fileName, fileExtension);

                    // Checks if local path exists, which indicates the file has been downloaded
                    //localFilePath += l "/" + objectName
                    //model cant be same name
                    //if (Directory.Exists(localFilePath))
                    //{
                    //    //Loads local file
                    //    Debug.Log("Loading : " + fileName);

                    //    result(LoadFile(fileExtension, localFilename));
                    //    yield return null;
                    //}
                    //else
                    //{
                        Debug.Log("Downloading : " + fileName);
                        //    If local path doesn't exists, download the file and create the local folder

                        Directory.CreateDirectory(localFilePath);
                        yield return StartCoroutine(DownloadFile(url, index, fileExtension, localFilePath, localFilename, result));

                  //  }
                }


           //     }
            }

      

            //Searches inside a path and returns the first path of an asset loadable by TriLib
            private string GetReadableAssetPath(string path)
            {
                var supportedExtensions = AssetLoaderBase.GetSupportedFileExtensions();
                foreach (var file in Directory.GetFiles(path))
                {
                    var fileExtension = FileUtils.GetFileExtension(file);
                    if (supportedExtensions.Contains("*" + fileExtension + ";"))
                    {
                        return file;
                    }
                }

                foreach (var directory in Directory.GetDirectories(path))
                {
                    var assetPath = GetReadableAssetPath(directory);
                    if (assetPath != null)
                    {
                        return assetPath;
                    }
                }

                return null;
            }

            //Loads an existing local file
            private GameObject LoadFile(string fileExtension, string localFilename)
            {

                //load with tiltbrush instead
                if (isTiltBrushFile(localFilename))
                {
                    Debug.Log("Using Tilt Brush loader.");
                    return LoadFileWithTiltBrushToolkit(localFilename);
                }

                //Creates a new AssetLoader instance
                using (var assetLoader = new AssetLoader())
                {
                    //Checks if the URL is a ZIP file
                    if (fileExtension == ".zip")
                    {
#if TRILIB_USE_ZIP
                        var localFilePath = FileUtils.GetFileDirectory(localFilename);

                    //Gets the first asset loadable by TriLib on the folder
                    var assetPath = GetReadableAssetPath(localFilePath);
                    if (assetPath == null)
                    {
                        Debug.LogError("No TriLib readable file could be found on the given directory");
                        return null;
                    }

                    //Loads the found asset
                     return _loadedGameObject = assetLoader.LoadFromFile(assetPath);
                        //#else
                        //throw new Exception("Please enable TriLib ZIP loading");
#endif
                    }
                    else
                    {
                        progressCallback += DownloadingProgressShow;
                        isUnloading = true;
                        currentObjectBeingLoaded = localFilename;

                        //If the URL is not a ZIP file, loads the file inside the folder

                        //set up what to upload from url_files
                        AssetLoaderOptions assetloadingOptions = AssetLoaderOptions.CreateInstance();
                        // AssetAdvancedConfig.CreateConfig(AssetAdvancedPropertyClassNames.)
                        assetloadingOptions.UseOriginalPositionRotationAndScale = true;
                        assetloadingOptions.DontLoadMetadata = true;

                        //if(bytes.Length > 0)
                        //return _loadedGameObject = assetLoader.LoadFromMemory(bytes, localFilename, options: assetloadingOptions);//, assetloadingOptions);
                        //else
                        //    var loadGO =
                        return _loadedGameObject = assetLoader.LoadFromFile(localFilename, assetloadingOptions);//, progressCallback: progressCallback);
                    }

                    return null;
                    //Move camera away to fit the loaded object in view
                    //  Camera.main.FitToBounds(_loadedGameObject.transform, 3f);
                }
            }

            AssimpInterop.ProgressCallback progressCallback;
            public string currentObjectBeingLoaded;
            public bool isUnloading;
            public void DownloadingProgressShow(float amount)
            {
                downloadText.text = "UNLOADING progress  : " + currentObjectBeingLoaded  + amount.ToString("P");
             //   Debug.Log("Trilib unloading progress  : " + amount.ToString("P"));

                if (Mathf.Approximately(amount, 1f))
                {
                    progressCallback = null;
                    isUnloading = false;
                }
            }

            /* 
          * Reads through file contents to check if GLB is Tilt Brush-specific.
          * Reads line-by-line and stops after finding the beginning of the binary section.
          */
            private bool isTiltBrushFile(string filename)
            {
                int timeOut = 5000; // length of time in milliseconds to allow file scan
                int minNumCharactersToRead = 1000;

                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                string tiltBrushString1 = "\"generator\": \"Tilt Brush";
                string tiltBrushString2 = "tiltbrush.com/shaders/";
                string tiltBrushString3 = "GOOGLE_tilt_brush_material";
                int lengthOfLongestString = tiltBrushString1.Length;

                //for the first line, read in the substring we want at its full length.
                int startIndex = 0;
                int numCharactersToRead = lengthOfLongestString;

                bool isFirstLine = true;

                using (StreamReader reader = new StreamReader(filename))
                {
                    var buffer = new char[numCharactersToRead];
                    int numCharactersRead = numCharactersToRead;
                    while (reader.Peek() > -1)
                    {
                        reader.ReadBlock(buffer, startIndex, numCharactersToRead);

                        string bufferAsString = new string(buffer);

                        if (numCharactersRead >= minNumCharactersToRead && watch.ElapsedMilliseconds > timeOut)
                        {
                            watch.Stop();
                            Debug.Log($"Hit time-out of {timeOut} ms before finding string. Read {numCharactersRead} total characters.");
                            return false;
                        }

                        if (bufferAsString.Contains("}") && bufferAsString.Contains("BIN"))
                        { // reached the end of the JSON section
                            watch.Stop();
                            Debug.Log($"Encountered BIN section of file before finding Tilt Brush string. Took {watch.ElapsedMilliseconds} ms.");
                            return false;
                        }

                        if (bufferAsString.Contains(tiltBrushString1) ||
                            bufferAsString.Contains(tiltBrushString2) ||
                            bufferAsString.Contains(tiltBrushString3))
                        {
                            watch.Stop();
                            Debug.Log($"Detected Tilt Brush file in {watch.ElapsedMilliseconds} ms.");
                            return true;
                        }

                        if (isFirstLine)
                        {
                            startIndex = lengthOfLongestString - 1;
                            numCharactersToRead = 1;
                            isFirstLine = false;
                            continue;
                        }

                        // after the first line, shift buffer to the left one character so we read the next substring.
                        Array.Copy(buffer, 1, buffer, 0, buffer.Length - 1);
                        numCharactersRead += 1;
                    }

                    watch.Stop();
                    Debug.Log($"Did not find Tilt Brush string in entire file. Took {watch.ElapsedMilliseconds} ms");
                    return false;
                }
            }

            private GameObject LoadFileWithTiltBrushToolkit(string localFilename)
            {
                _loadedGameObject = Glb2Importer.ImportTiltBrushAsset(localFilename);
                return _loadedGameObject;
            }

            

            // A quirk of Unity’s memory management in WebGL is that the garbage collector 
            //can only run in between frames instead of during a frame.If you allocate too much memory(even if it becomes garbage) 
            //in the middle of a frame, you’ll hit that game-ending out of memory exception.
            //https://blog.kongregate.com/managing-game-loading-memory-in-webgl-with-unity/
            //public IEnumerator LoadBytesProcess(UnityWebRequest UWR)
            //{
            ////    isLoading = true;
            //    Debug.Log("STARTED");
            //    UWR.SendWebRequest();
            //    //  var i = 0;
            //    while (!UWR.isDone)
            //    {
            //        downloadText.text = "Trilib DOWNLOADING progress  : " + UWR.downloadProgress.ToString("P");
            //        Debug.Log("BYTES Downloaded : " + UWR.downloadedBytes);
            //        yield return null;

            //    }
            //    //while (UWR.isDone)//downloadedBytes < sizeOfAsset -100)
            //    //{
            //    //    i++;

            //    //    if (i > 200)
            //    //    {
            //    //        i = 0;
            //    //        yield return new WaitForSecondsRealtime(0.05f);
            //    //    }

            //    //    downloadText.text = "Trilib DOWNLOADING progress  : " + UWR.downloadProgress.ToString("P");
            //    //    Debug.Log("BYTES Downloaded : " + UWR.downloadedBytes);


            //    //}

            ////    yield return null;


            //}

            public bool isLoading;

            private void OnLoadComplete()
            {
                isLoading = false;
                Debug.Log("Load Complete!");
            }

            //Downloads a file to a local path or extract all ZIP file contents to the local path in case of ZIP files, then loads the file
            private IEnumerator DownloadFile(string url, int index, string fileExtension, string localFilePath, string localFilename, System.Action<GameObject> result = null)
            {

                //load with tiltbrush instead
                //if (isTiltBrushFile(localFilename))
                //{
                //    Debug.Log("Tilt Brush file detected. Using Tilt Brush loader.");
                //    return LoadFileWithTiltBrushToolkit(localFilename);
                //}
                //   _fileDownloaders[index] = UnityWebRequest.Get(url);
                _fileDownloaders[index] = UnityWebRequest.Get(url); //new UnityWebRequest(url);

                //get size of asset first to allocate what is needed
                long sizeOfAsset = 0;
                yield return StartCoroutine(GetFileSize(url, (size) => { sizeOfAsset = size;
      
     }));



                _fileDownloaders[index].method = UnityWebRequest.kHttpVerbGET;
                var dh = new DownloadHandlerFile(localFilename);
                dh.removeFileOnAbort = true;
                _fileDownloaders[index].downloadHandler = dh;

                _fileDownloaders[index].SendWebRequest();

                string AssetName = url_List.url_list[index].name;

                int i = 0;
                while (!_fileDownloaders[index].isDone)//downloadedBytes < sizeOfAsset -100)
                {
                    //i++;

                    //if (i > 20)
                    //{
                    //    i = 0;
                    //    yield return new WaitForSecondsRealtime(0.05f);
                    //}

                    downloadText.text = "DOWNLOADING progress  :  " + AssetName + "   " + _fileDownloaders[index].downloadProgress.ToString("P");
                    Debug.Log("BYTES Downloaded : " + AssetName + "  " + _fileDownloaders[index].downloadedBytes);

                    //per frame instead?
                    yield return new WaitForSecondsRealtime(0.05f);

                }
                if (_fileDownloaders[index].isNetworkError || _fileDownloaders[index].isHttpError)
                    Debug.Log(_fileDownloaders[index].error);
                else//if searching for a local file from indexDB look in here?
                    Debug.Log("Download saved to: " + localFilename.Replace("/", "\\") + "\r\n" + _fileDownloaders[index].error);
                //_fileDownloaders[index].downloadHandler = new CustomAssetBundleDownloadHandler(new byte[sizeOfAsset], localFilename);

                //_fileDownloaders[index].SendWebRequest();

                //int i = 0;
                //string AssetName = url_List.url_list[index].name;

                //while (!_fileDownloaders[index].isDone)//downloadedBytes < sizeOfAsset -100)
                //{
                //    i++;

                //    if (i > 20)
                //    {
                //        i = 0;
                //        yield return new WaitForSecondsRealtime(0.05f);
                //    }

                //    downloadText.text = "DOWNLOADING progress  :  " + AssetName + "   " +_fileDownloaders[index].downloadProgress.ToString("P");
                // Debug.Log("BYTES Downloaded : " + AssetName + "  " + _fileDownloaders[index].downloadedBytes);


                //      }
                //UnityGameLoader.LoadManager.instance.RegisterEnumerator(LoadBytesProcess(_fileDownloaders[index]));
                //UnityGameLoader.LoadManager.instance.LoadRegistered(OnLoadComplete);

                ////  UnityGameLoader.LoadManager.instance.



                //yield return new WaitUntil( () => { if (_fileDownloaders[index].isDone) return true; return false; } );


                //Debug.Log("END");
                //                UnityGameLoader.LoadManager.instance.RegisterEnumerator(LoadBytesProcess(_fileDownloaders[index]);
                //     yield return StartCoroutine(LoadBytesProcess(_fileDownloaders[index]));

                //    UnityWebRequestAsyncOperation uWRAO =






                if (fileExtension == ".zip")
                {
#if TRILIB_USE_ZIP
                    //  using (var memoryStream = new MemoryStream(uwr.downloadHandler.data))
                    using (var memoryStream = new MemoryStream(_fileDownloaders[index].downloadHandler.data))
                {
                    UnzipFromStream(memoryStream, localFilePath);
                }
#else
                    throw new Exception("Please enable TriLib ZIP loading");
#endif
                }
                Debug.Log($"Successfully downloaded asset #{index}, size {_fileDownloaders[index].downloadedBytes} bytes.");

                //Directory.CreateDirectory(localFilePath);
                //File.WriteAllBytes(localFilename, _fileDownloaders[index].downloadHandler.data);

                if (result != null)
                {

                    result(LoadFile(fileExtension, localFilename));
                }
                else
                    LoadFile(fileExtension, localFilename);

             //   uwr = null;
                _fileDownloaders[index] = null;


             
            }

#if TRILIB_USE_ZIP
            //Helper function to extract all ZIP file contents to a local folder
            private void UnzipFromStream(Stream zipStream, string outFolder)
        {
#if !UNITY_EDITOR && (NETFX_CORE || NET_4_6 || NET_STANDARD_2_0) && !ENABLE_IL2CPP && !ENABLE_MONO
            var zipFile = new ZipArchive(zipStream, ZipArchiveMode.Read);
            foreach (ZipArchiveEntry zipEntry in zipFile.Entries)
            {
                var zipFileStream = zipEntry.Open();
#else
                var zipFile = new ZipFile(zipStream);
            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }
                var zipFileStream = zipFile.GetInputStream(zipEntry);
#endif
                var entryFileName = zipEntry.Name;
                var buffer = new byte[4096];
                var fullZipToPath = Path.Combine(outFolder, entryFileName);
                var directoryName = Path.GetDirectoryName(fullZipToPath);
                if (!string.IsNullOrEmpty(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                var fileName = Path.GetFileName(fullZipToPath);
                if (fileName.Length == 0)
                {
                    continue;
                }
                using (var streamWriter = File.Create(fullZipToPath))
                {
#if !UNITY_EDITOR && (NETFX_CORE || NET_4_6 || NET_STANDARD_2_0) && !ENABLE_IL2CPP && !ENABLE_MONO
                    zipFileStream.CopyTo(streamWriter);
#else
                    ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipFileStream, streamWriter, buffer);
#endif
                }
            }
        }
#endif

            IEnumerator GetFileSize(string url, Action<long> resut)
            {
                UnityWebRequest uwr = UnityWebRequest.Head(url);
                yield return uwr.SendWebRequest();
                string size = uwr.GetResponseHeader("Content-Length");

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log("Error While Getting Length: " + uwr.error);
                    if (resut != null)
                        resut(-1);
                }
                else
                {
                    if (resut != null)
                        resut(Convert.ToInt64(size));
                }
            }



        }




    }





}
#pragma warning restore 649


