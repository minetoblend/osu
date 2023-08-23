// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.IO;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Database;
using osu.Game.Extensions;

namespace osu.Game.Online.Editor
{
    public class MultiplayerEditorBeatmapSerializer
    {
        private readonly Storage userFileStorage;

        public MultiplayerEditorBeatmapSerializer(Storage storage)
        {
            userFileStorage = storage.GetStorageForDirectory(@"files");
        }

        public Dictionary<string, byte[]> SerializeBeatmapFiles(BeatmapSetInfo model)
        {
            var fileDict = new Dictionary<string, byte[]>();

            foreach (var file in model.Files)
            {
                using (var stream = GetFileContents(model, file))
                {
                    if (stream == null)
                    {
                        Logger.Log($"File {file.Filename} is missing in local storage and will not be included in the export", LoggingTarget.Database);
                        // anyFileMissing = true;
                        continue;
                    }

                    using (var reader = new BinaryReader(stream))
                    {
                        fileDict[file.Filename] = reader.ReadBytes((int)stream.Length);
                        ;
                    }
                }
            }

            return fileDict;
        }

        protected virtual Stream? GetFileContents(BeatmapSetInfo model, INamedFileUsage file) => userFileStorage.GetStream(file.File.GetStoragePath());

        public string SerializeBeatmap(IBeatmap beatmap)
        {
            using (var writer = new StringWriter())
            {
                new LegacyBeatmapEncoder(beatmap, null).Encode(writer);

                return writer.ToString();
            }
        }
    }
}
