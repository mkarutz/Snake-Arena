//
//  AAShare.m
//  activitysheetplugin
//
//  Created by Gilbert Annthony Barouch - App Advisory on 2016.05.18
//  Copyright (c) 2016 App Advisory. All rights reserved.
//

#import "AAShare.h"
#import <UIKit/UIKit.h>

#define SYSTEM_VERSION_EQUAL_TO(v)                  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedSame)
#define SYSTEM_VERSION_GREATER_THAN(v)              ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedDescending)
#define SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN(v)                 ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN_OR_EQUAL_TO(v)     ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedDescending)
/* 1.1 */

@interface AAShare (){
UIActivityIndicatorView *spinner;
}
@end

@implementation AAShare


- (void)_presentActivitySheetWithData :(id)data{
    
    if ( [self isVersionSupported] == NO)
        return;
    
    UIActivityViewController *av = [[UIActivityViewController alloc]initWithActivityItems:[[NSArray alloc] initWithObjects:data,nil] applicationActivities:nil];
    [[[UIApplication sharedApplication]keyWindow].rootViewController presentViewController:av animated:YES completion:^{
          [self dismissLoadingSprite];
    }];
    
    if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
    {
        av.popoverPresentationController.sourceView = [[UIApplication sharedApplication]keyWindow].rootViewController.view;
        av.popoverPresentationController.sourceRect = CGRectMake(100,100,5,5);
    }
    
    spinner = [[UIActivityIndicatorView alloc]initWithFrame: [[UIApplication sharedApplication]keyWindow].frame];
    [spinner startAnimating];
    [[[UIApplication sharedApplication]keyWindow].rootViewController.view addSubview:spinner];
}
- (void)_presentActivitySheetWithArray : (NSArray*) data{
    
    if ( [self isVersionSupported] == NO)
        return;
    
    UIActivityViewController *av = [[UIActivityViewController alloc]initWithActivityItems:data applicationActivities:nil];
    [[[UIApplication sharedApplication]keyWindow].rootViewController presentViewController:av animated:YES completion:^{
        [self dismissLoadingSprite];
    }];
    
    if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
    {
        av.popoverPresentationController.sourceView = [[UIApplication sharedApplication]keyWindow].rootViewController.view;
        av.popoverPresentationController.sourceRect = CGRectMake(100,100,5,5);
    }
    
    spinner = [[UIActivityIndicatorView alloc]initWithFrame: [[UIApplication sharedApplication]keyWindow].frame];
    [spinner startAnimating];
    [[[UIApplication sharedApplication]keyWindow].rootViewController.view addSubview:spinner];

}

- (void)dismissLoadingSprite{
    NSLog(@"Dismissing loading sprite");
    [spinner stopAnimating];
    [spinner removeFromSuperview];
}

-(BOOL)isVersionSupported{
    
    if ( SYSTEM_VERSION_LESS_THAN(@"6.0") ){
        NSLog(@"This version of iOS is not supported activity sheet is only present with devices iOS 6+!");
        UIAlertView *av = [[UIAlertView alloc]initWithTitle:@"Share is not supported" message:@"Share is not supported on devices with software older than iOS 6, please update to the most current iOS software if your device is eligible to use sharing!" delegate:nil cancelButtonTitle:@"Ok" otherButtonTitles:nil, nil];
        
        [av show];
        return NO;
    }
    else{
        
        return YES;
    }
    
}

@end

extern "C"
{
    AAShare *social;
    
    void presentActivitySheetWithString(Byte *socialData,int _length){

      social = [[AAShare alloc]init];
      NSUInteger n = (unsigned long) _length;
      NSLog(@"Length is %lu",(unsigned long)n);
      NSData *d = [[NSData alloc]initWithBytes:socialData length:n*sizeof(char)];
      NSLog(@"data length %lu",(unsigned long)[d length]);
      NSString *s = [[NSString alloc]initWithData:(NSData*)d encoding:NSUTF8StringEncoding];
      [social _presentActivitySheetWithData:s];
    }
    void presentActivitySheetWithImage(Byte *socialData,int _length){
        social = [[AAShare alloc]init];
        NSUInteger n = (unsigned long) _length;
        NSLog(@"Length is %lu",(unsigned long)n);
        NSData *d = [[NSData alloc]initWithBytes:socialData length:n];
        UIImage *img = [[UIImage alloc]initWithData:d];
        [social _presentActivitySheetWithData:img];
    }
    void presentActivitySheetWithImageAndString(char *message,Byte *imgData,int _length){
  
        social = [[AAShare alloc]init];
        NSUInteger n = (unsigned long)_length;
        NSData *_imgData =[[NSData alloc]initWithBytes:imgData length:n];
//        UIImage *img = [[UIImage alloc]initWithData:_imgData];
        NSString *_message = [NSString stringWithUTF8String:message];
        NSArray *data = [[NSArray alloc]initWithObjects:_imgData,_message, nil];
        [social _presentActivitySheetWithArray:data];
        
    }
}


/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/


