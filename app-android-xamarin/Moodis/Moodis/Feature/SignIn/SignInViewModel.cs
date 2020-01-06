using Android.Arch.Lifecycle;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Moodis.Constants.Enums;
using Moodis.Extensions;
using Moodis.Feature.Login;
using Moodis.Network;
using Moodis.Network.Face;
using Moodis.Network.Requests;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Moodis.Feature.SignIn
{
    public class SignInViewModel : ViewModel
    {
        public static User currentUser;

        public async Task<Response> Authenticate(string username, string password)
        {
            try
            {
                char[] array = password.ToCharArray();
                Array.Reverse(array);
                password = new string(array);
                currentUser = await API.UserEndpoint.LoginUser(new LoginRequest(username, password));
                return Response.OK;
            }
            catch (ApiException ex)
            {
                var statusCode = ex.StatusCode;
                return statusCode switch
                {
                    HttpStatusCode.NotFound => Response.BadCredentials,
                    HttpStatusCode.BadRequest => Response.BadCredentials,
                    _ => Response.ApiError
                };
            }
        }

        public async Task<Response> AuthenticateWithFace(string imagePath, Action<DetectedFace> callback)
        {
            List<DetectedFace> detectedFaces = null;
            void setFace(List<DetectedFace> faces) => detectedFaces = faces;
            DetectedFace face;

            List<Person> identifiedPersons = null;
            try
            {
                imagePath.RotateImage();
                identifiedPersons = await Face.Instance.IdentifyPersons(imagePath, setFace) as List<Person>;
            }
            catch
            {
                if (detectedFaces.IsNullOrEmpty())
                {
                    return Response.FaceNotDetected;
                }
                else
                {
                    return Response.ApiError;
                }
            }

            if (identifiedPersons.IsNullOrEmpty())
            {
                return Response.UserNotFound;
            }

            try
            {
                currentUser = await API.UserEndpoint.GetUser(identifiedPersons.ToArray()[0].PersonId.ToString());
            }
            catch (ApiException ex)
            {
                return ex.StatusCode switch {
                    HttpStatusCode.NotFound => Response.UserNotFound,
                    _ => Response.ApiError
                };
            }

            face = detectedFaces.ToArray()[0];
            callback(face);
            return Response.OK;
        }
    }
}
