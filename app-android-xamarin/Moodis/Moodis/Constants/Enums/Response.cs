﻿namespace Moodis.Constants.Enums
{
    public enum Response
    {
        OK,
        GeneralError,
        ApiError,
        FaceNotDetected,
        //Registration-specific errors
        UserExists,
        RegistrationDone,
        //SignIn Errors
        UserNotFound,
    }
}
