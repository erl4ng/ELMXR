﻿/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.Voice;
using Meta.WitAi.Json;
using Meta.WitAi.Configuration;
using Meta.WitAi.Requests;

namespace Oculus.Voice.Bindings.Android
{
    /// <summary>
    /// A class generated by VoiceSDKImpl to track voice request calls
    /// </summary>
    public class VoiceSDKImplRequest : VoiceServiceRequest
    {
        public VoiceSDKBinding Service { get; private set; }
        public bool Immediately { get; private set; }

        #region CALLS
        public VoiceSDKImplRequest(VoiceSDKBinding newService, NLPRequestInputType newInputType, bool newImmediately, WitRequestOptions newOptions,
            VoiceServiceRequestEvents newEvents) : base(newInputType, newOptions, newEvents)
        {
            Service = newService;
            Immediately = newImmediately;
        }

        protected override void HandleAudioActivation()
        {
            if (Immediately)
            {
                Service.ActivateImmediately(Options);
            }
            else
            {
                Service.Activate(Options);
            }
            SetAudioInputState(VoiceAudioInputState.On);
        }

        protected override void HandleAudioDeactivation()
        {
            Service.Deactivate(Options.RequestId);
            SetAudioInputState(VoiceAudioInputState.Off);
        }

        protected override void HandleSend()
        {
            if (InputType == NLPRequestInputType.Text)
            {
                Service.Activate(Options.Text, Options);
            }
        }

        protected override void HandleCancel()
        {
            Service.DeactivateAndAbortRequest(Options.RequestId);
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Called audio is now listening
        /// </summary>
        public void HandleStartListening()
        {
            //NOT YET IMPLEMENTED
        }

        /// <summary>
        /// Called audio is no longer listening
        /// </summary>
        public void HandleStopListening()
        {
            //NOT YET IMPLEMENTED
        }
        /// <summary>
        /// Callback when in progress response data has been received
        /// </summary>
        /// <param name="responseJson">The unparsed json data</param>
        public void HandlePartialResponse(string responseJson)
        {
            var responseData = WitResponseNode.Parse(responseJson);
            HandlePartialResponse(responseData, null);
        }

        /// <summary>
        /// Called for partial transcription of text
        /// </summary>
        public void HandlePartialTranscription(string transcription)
        {
            ApplyTranscription(transcription, false);
        }

        /// <summary>
        /// Called for final transcription of text
        /// </summary>
        public void HandleFullTranscription(string transcription)
        {
            ApplyTranscription(transcription, true);
        }

        /// <summary>
        /// Transmission began
        /// </summary>
        public void HandleTransmissionBegan()
        {
            if (InputType == NLPRequestInputType.Audio)
            {
                Send();
            }
        }

        /// <summary>
        /// Called when an error message has been received
        /// </summary>
        public void HandleCanceled()
        {
            HandleCancel();
        }

        /// <summary>
        /// Called when an error message has been received
        /// </summary>
        public void HandleError(string error, string message, string errorBody)
        {
            HandleFailure($"{error} - {message}");
        }

        /// <summary>
        /// Callback when final response data has been received
        /// </summary>
        /// <param name="responseJson">The unparsed json data</param>
        public void HandleResponse(string responseJson)
        {
            var responseData = WitResponseNode.Parse(responseJson);
            HandleFinalResponse(responseData, null);
        }
        #endregion
    }
}
