using Game;
using Game.SceneFlow;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace LegacyFlavour.MonoBehaviours
{
    /// <summary>
    /// Mono behaviour for running a snow detection compute shader
    /// </summary>
    public class SnowDetector : MonoBehaviour
    {
        public bool Disable
        {
            get;
            set;
        }

        public Action<float> OnUpdate;

        private ComputeShader snowCoverageShader;
        private ComputeBuffer resultBuffer;
        private RenderTexture inputTexture;

        const float RUN_FREQUENCY = 15f;

        private bool isRunning;
        private float endTime;
        private float lastSnowPercent;

        private void Start( )
        {
            snowCoverageShader = LoadFromAssetBundle( );
        }

        private void Update( )
        {
            if ( Disable || !GameManager.instance.gameMode.IsGame( ) )
                return;

            if ( TryGetSnowTexture() && !isRunning && Time.time >= endTime + RUN_FREQUENCY )
                Run( );
        }

        /// <summary>
        /// Try to get the snow RenderTexture for the game
        /// </summary>
        /// <returns></returns>
        private bool TryGetSnowTexture( )
        {
            if ( inputTexture != null )
                return true;

            inputTexture = ( RenderTexture ) Shader.GetGlobalTexture( "_SnowMap" );
            return inputTexture != null;
        }

        /// <summary>
        /// Run the snow detector
        /// </summary>
        private void Run( )
        {
            resultBuffer = new ComputeBuffer( 2, sizeof( float ), ComputeBufferType.Default );

            var kernelHandle = snowCoverageShader.FindKernel( "CSMain" );
            snowCoverageShader.SetTexture( kernelHandle, "InputTexture", inputTexture );
            snowCoverageShader.SetBuffer( kernelHandle, "Result", resultBuffer );

            snowCoverageShader.Dispatch( kernelHandle, inputTexture.width / 8, inputTexture.height / 8, 1 );

            AsyncGPUReadback.Request( resultBuffer, OnCompleteReadback );
            isRunning = true;
        }

        /// <summary>
        /// Readback the snow detection result
        /// </summary>
        /// <param name="request"></param>
        private void OnCompleteReadback( AsyncGPUReadbackRequest request )
        {
            if ( request.hasError )
            {
                Debug.LogError( "GPU readback error detected." );
                return;
            }

            var result = request.GetData<uint>( ).ToArray( );
            var snowCoveragePercentage = ( float ) result[0] / ( inputTexture.width * inputTexture.height * 100f ) * 100f;

            if ( snowCoveragePercentage != lastSnowPercent )
            {
                lastSnowPercent = snowCoveragePercentage;
                //Debug.Log( "Snow Coverage Percentage: " + snowCoveragePercentage + "%" );

                OnUpdate?.Invoke( snowCoveragePercentage );
            }

            resultBuffer.Release( );
            isRunning = false;
            endTime = Time.time;
        }

        /// <summary>
        /// Load our compute shader from an asset bundle embedded resource
        /// </summary>
        /// <returns></returns>
        private ComputeShader LoadFromAssetBundle( )
        {
            using ( var stream = typeof( SnowDetector ).Assembly.GetManifestResourceStream( "LegacyFlavour.Resources.calculatesnow" ) )
            {
                if ( stream == null )
                {
                    Debug.LogError( "Failed to load embedded resource." );
                    return null;
                }

                var assetBytes = new byte[stream.Length];
                stream.Read( assetBytes, 0, assetBytes.Length );

                var myLoadedAssetBundle = AssetBundle.LoadFromMemory( assetBytes );
                if ( myLoadedAssetBundle == null )
                {
                    Debug.LogError( "Failed to load AssetBundle from memory." );
                    return null;
                }

                //var assetNames = myLoadedAssetBundle.GetAllAssetNames( );
                //foreach ( var name in assetNames )
                //{
                //    Debug.Log( name );
                //}

                // Load an asset from the bundle
                var loadedShader = myLoadedAssetBundle.LoadAsset<ComputeShader>( "assets/compute/calculatesnow.compute" );

                if ( loadedShader == null )
                {
                    Debug.LogError( "Failed to load the compute shader from the AssetBundle." );
                    return null;
                }
                myLoadedAssetBundle.Unload( false );
                return loadedShader;
            }
        }

        /// <summary>
        /// Create the snow detector instance
        /// </summary>
        /// <returns></returns>
        public static SnowDetector Create( )
        {
            var obj = GameObject.Find( "LegacyFlavour_SnowDetector" );

            if ( obj != null )
                return obj.GetComponent<SnowDetector>( );

            var gameObject = new GameObject( "LegacyFlavour_SnowDetector" );
            var instance = gameObject.AddComponent<SnowDetector>( );
            DontDestroyOnLoad( gameObject );
            return instance;
        }
    }
}
