﻿using BulkResponseParsingDemo.BulkResponseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BulkResponseParsingDemo
{
    public static class BulkResponseParser
    {
        public static (bool Success, IReadOnlyCollection<string> Errors) ParseResponse(Stream stream, CancellationToken cancellationToken = default)
        {
            // C# 8 FTW!! :-)
            using var streamReader = new StreamReader(stream);
            using var reader = new JsonTextReader(streamReader);

            JsonSerializer serializer = new JsonSerializer();

            var response = serializer.Deserialize<BulkResponse>(reader);

            if (!response.Errors)
                return (true, Array.Empty<string>());

            var errors = response.Items
                .Where(x => x.Index.Status == 400)
                .Select(x => x.Index.Id)
                .ToArray();

            return (false, errors);
        }         
    }
}
