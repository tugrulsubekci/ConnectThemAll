//
//  AUStateAPI.h
//  AirbridgeUnity
//
//  Created by WOF on 29/11/2019.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface AUStateAPI : NSObject

+ (AUStateAPI*) instance;

- (void) setUserID:(nullable const char*)idChars;
- (void) setUserEmail:(nullable const char*)emailChars;
- (void) setUserPhone:(nullable const char*)phoneChars;
- (void) addUserAliasWithKey:(nullable const char*)keyChars
                       value:(nullable const char*)valueChars;
- (void) setSDKDevelopmentPlatform:(nullable const char*)platformChars;

@end

void native_setUserID(const char* __nullable ID);
void native_setUserEmail(const char* __nullable email);
void native_setUserPhone(const char* __nullable phone);
void native_addUserAlias(const char* __nullable key, const char* __nullable value);
void native_addUserAttributesWithInt(const char* __nullable key, int value);
void native_addUserAttributesWithLong(const char* __nullable key, long long value);
void native_addUserAttributesWithFloat(const char* __nullable key, float value);
void native_addUserAttributesWithBOOL(const char* __nullable key, BOOL value);
void native_addUserAttributesWithString(const char* __nullable key, const char* __nullable value);
void native_clearUserAttributes(void);
void native_expireUser(void);

NS_ASSUME_NONNULL_END
