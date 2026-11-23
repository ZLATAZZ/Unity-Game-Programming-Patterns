using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace CharacterModifierBytecodeDemo.Bytecode
{
    public sealed class BytecodeProgramLoader
    {
        private readonly string _programFolderName;

        public BytecodeProgramLoader(string programFolderName)
        {
            if (string.IsNullOrWhiteSpace(programFolderName))
            {
                throw new ArgumentException("Program folder name cannot be empty.", nameof(programFolderName));
            }

            _programFolderName = programFolderName;
        }

        public IEnumerator LoadSource(
            string fileName,
            Action<string> onLoaded,
            Action<string> onFailed)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                onFailed?.Invoke("Program file name cannot be empty.");
                yield break;
            }

            string safeFileName = Path.GetFileName(fileName);

            if (!string.Equals(fileName, safeFileName, StringComparison.Ordinal))
            {
                onFailed?.Invoke("Program file name cannot contain a path.");
                yield break;
            }

            string path = Path.Combine(
                Application.streamingAssetsPath,
                _programFolderName,
                safeFileName);

            string requestUrl = ToRequestUrl(path);

            using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onFailed?.Invoke(
                        $"Failed to load '{safeFileName}': {request.error}\n{requestUrl}");
                    yield break;
                }

                onLoaded?.Invoke(request.downloadHandler.text);
            }
        }

        public string GetDisplayPath(string fileName)
        {
            return Path.Combine(
                Application.streamingAssetsPath,
                _programFolderName,
                Path.GetFileName(fileName));
        }

        private static string ToRequestUrl(string path)
        {
            string normalizedPath = path.Replace('\\', '/');

            if (normalizedPath.Contains("://"))
            {
                return normalizedPath;
            }

            return new Uri(normalizedPath).AbsoluteUri;
        }
    }
}
