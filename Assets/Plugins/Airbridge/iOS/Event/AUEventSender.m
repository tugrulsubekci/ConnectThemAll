//
//  AUEventSender.m
//  AirbridgeUnity
//
//  Created by WOF on 2019/11/29.
//  Copyright © 2019 ab180. All rights reserved.
//

#import "AUEventSender.h"

#import <AirBridge/ABUserEvent.h>
#import <AirBridge/ABEcommerceEvent.h>

#import "AUEventKeys.h"

@implementation AUEventSender

- (void) send:(ABInAppEvent*)event category:(NSString*)category {
    if ([event isKindOfClass:ABUserEvent.class]
        && ([category isEqualToString:CATEGORY__SIGN_IN]
            || [category isEqualToString:CATEGORY__SIGN_UP]
            || [category isEqualToString:CATEGORY__SIGN_OUT]))
    {
        [self sendUserEvent:(ABUserEvent*) event category:category];
    } else if ([event isKindOfClass:ABEcommerceEvent.class]
               && ([category isEqualToString:CATEGORY__VIEW_HOME]
                   || [category isEqualToString:CATEGORY__VIEW_SEARCH_RESULT]
                   || [category isEqualToString:CATEGORY__VIEW_PRODUCT_LIST]
                   || [category isEqualToString:CATEGORY__VIEW_PRODUCT_DETAILS]
                   || [category isEqualToString:CATEGORY__ADD_TO_CART]
                   || [category isEqualToString:CATEGORY__PURCHASE]))
    {
        [self sendEcommerceEvent:(ABEcommerceEvent*) event category:category];
    } else {
        [self sendCustomEvent:event category:category];
    }
}

- (void) sendUserEvent:(ABUserEvent*)event category:(NSString*)category {
    if ([category isEqualToString:CATEGORY__SIGN_IN]) {
        [event sendSignin];
    } else if ([category isEqualToString:CATEGORY__SIGN_UP]) {
        [event sendSignup];
    } else if ([category isEqualToString:CATEGORY__SIGN_OUT]) {
        [event expireUser];
    }
}

- (void) sendEcommerceEvent:(ABEcommerceEvent*)event category:(NSString*)category {
    if ([category isEqualToString:CATEGORY__VIEW_HOME]) {
        [event sendViewHome];
    } else if ([category isEqualToString:CATEGORY__VIEW_SEARCH_RESULT]) {
        [event sendViewSearchResult];
    } else if ([category isEqualToString:CATEGORY__VIEW_PRODUCT_LIST]) {
        [event sendViewProductList];
    } else if ([category isEqualToString:CATEGORY__VIEW_PRODUCT_DETAILS]) {
        [event sendViewProductDetail];
    } else if ([category isEqualToString:CATEGORY__ADD_TO_CART]) {
        [event sendAddProductToCart];
    } else if ([category isEqualToString:CATEGORY__PURCHASE]) {
        [event sendCompleteOrder];
    }
}

- (void) sendCustomEvent:(ABInAppEvent*)event category:(NSString*)category {
    [event send];
}

@end
