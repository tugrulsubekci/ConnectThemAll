//
//  AUPlacementAPI.m
//  AirbridgeUnity
//
//  Created by WOF on 29/11/2019.
//

#import "AUPlacementAPI.h"

#import <AirBridge/AirBridge.h>

#import "AUGet.h"
#import "AUConvert.h"

@interface AUPlacementAPI (Internal)

+ (void) setInstance:(AUPlacementAPI*)input;
- (instancetype)initWithPlacementAPI:(ABPlacement*)placementAPI;

@end

@implementation AUPlacementAPI {
    ABPlacement* placementAPI;
}

static AUPlacementAPI* instance;

//
// singleton
//

+ (AUPlacementAPI*) instance {
    if (instance == nil) {
        instance = [[AUPlacementAPI alloc] init];
    }
    
    return instance;
}

+ (void) setInstance:(AUPlacementAPI*)input {
    instance = input;
}

//
// init
//

- (instancetype)init {
    ABPlacement* placementAPI = AirBridge.placement;
    
    return [self initWithPlacementAPI:placementAPI];
}

- (instancetype)initWithPlacementAPI:(ABPlacement*)placementAPI {
    self = [super init];
    if (!self) {
        return nil;
    }
    
    self->placementAPI = placementAPI;
    
    return self;
}

//
// method
//

- (void) click:(nullable const char*)trackingLinkChars
      deeplink:(nullable const char*)deeplinkChars
      fallback:(nullable const char*)fallbackChars
{
    NSString* trackingLink = [AUConvert stringFromChars:trackingLinkChars];
    NSString* deeplink = [AUConvert stringFromChars:deeplinkChars];
    NSString* fallback = [AUConvert stringFromChars:fallbackChars];
    
    [placementAPI click:trackingLink deeplink:deeplink fallback:fallback];
}

- (void) impression:(nullable const char*)trackingLinkChars {
    NSString* trackingLink = [AUConvert stringFromChars:trackingLinkChars];
    
    [placementAPI impression:trackingLink];
}

@end

//
// unity method
//

void native_click(const char* __nullable trackingLinkChars,
           const char* __nullable deeplinkChars,
           const char* __nullable fallbackChars)
{
    [AUPlacementAPI.instance click:trackingLinkChars deeplink:deeplinkChars fallback:fallbackChars];
}

void native_impression(const char* __nullable trackingLinkChars)
{
    [AUPlacementAPI.instance impression:trackingLinkChars];
}
