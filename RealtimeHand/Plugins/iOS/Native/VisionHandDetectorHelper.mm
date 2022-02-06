// Copyright (c) Olivier Goguel 2021
// Licensed under the MIT License.

#import <UnityFramework/UnityFramework-Swift.h>
#import <ARKit/ARKit.h>
#import "UnityXRNativePtrs.h"

extern "C" {

    VisionHandDetector *helper ;
    char* convertNSStringToCString(const NSString* nsString)
    {
        if (nsString == NULL)
            return NULL;

        const char* nsStringUtf8 = [nsString UTF8String];
        char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
        strcpy(cString, nsStringUtf8);
        return cString;
    }

    void VisionHandDetectorCreate(void* session) {
        UnityXRNativeSession_1* unityXRSession = (UnityXRNativeSession_1*) session;
        ARSession* sess = (__bridge ARSession*)unityXRSession->sessionPtr;
        helper = [[VisionHandDetector alloc] initWithArsession:sess];
        [helper setUp ];
    }

    void VisionHandDetectorRelease() {
        helper = nil;
    }

    char* Process() {
        if (helper == nil) {
            return nil;
        }
        return convertNSStringToCString([helper process]);
    }
}

