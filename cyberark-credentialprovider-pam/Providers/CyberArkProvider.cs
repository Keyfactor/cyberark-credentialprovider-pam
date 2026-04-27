// Copyright 2026 Keyfactor
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public abstract class CyberArkProvider
    {
        protected ILogger Logger;
        
        protected const string InitializationInfoDictionaryName = "initialization info";
        protected const string InstanceParametersDictionaryName = "instance parameter";

        protected string GetRequiredValue(Dictionary<string, string> dict, string key, string dictionaryName)
        {
            if (!dict.ContainsKey(key)
                || string.IsNullOrWhiteSpace(dict[key]))
            {
                string error = $"Required {dictionaryName} field {key} was missing a value or was not defined as expected in dictionary.";
                Logger.LogError(error);
                throw new ArgumentException(error);
            }
            return dict[key];
        }
    }
}
