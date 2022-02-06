// Copyright (c) Olivier Goguel 2022
// Licensed under the MIT License.

import UIKit
import ARKit
import Vision
import Foundation

@available(iOS 14.0, *)
public class VisionHandDetector : NSObject {
    
    private let serialQueue = DispatchQueue(label: "systems.expose.visionhand")
    private var visionRequests = [VNRequest]()
    private var timer = Timer()
   
    var session:ARSession?
    var handPoseRequest:VNDetectHumanHandPoseRequest
    
    var nameConversions: [VNHumanHandPoseObservation.JointName: String] = [
            .wrist: "wrist",
            .thumbCMC: "thumbCMC",
            .thumbMP: "thumbMP",
            .thumbIP: "thumbIP",
            .thumbTip: "thumbTip",
            .indexMCP: "indexMCP",
            .indexPIP: "indexPIP",
            .indexDIP: "indexDIP",
            .indexTip: "indexTip",
            .middleMCP: "middleMCP",
            .middlePIP: "middlePIP",
            .middleDIP: "middleDIP",
            .middleTip: "middleTip",
            .ringMCP: "ringMCP",
            .ringPIP: "ringPIP",
            .ringDIP: "ringDIP",
            .ringTip: "ringTip",
            .littleMCP: "littleMCP",
            .littlePIP: "littlePIP",
            .littleDIP: "littleDIP",
            .littleTip: "littleTip"
            ]
        
    @objc public init(arsession: ARSession) {
        self.session = arsession
        self.handPoseRequest = VNDetectHumanHandPoseRequest()
    }

    @objc public func setUp() {
        handPoseRequest.usesCPUOnly = false;
        handPoseRequest.maximumHandCount = 1
    }
    
    @objc public func process() -> String {
        
        var msg:String = ""
        
        guard let pixbuff : CVPixelBuffer = (session?.currentFrame?.capturedImage)
        else {
            return msg;
        }
        
        // TODO Manage orientation
        let deviceOrientation = CGImagePropertyOrientation.up
        let imageRequestHandler = VNImageRequestHandler(cvPixelBuffer: pixbuff, orientation: deviceOrientation,options: [:])
        
        do {
            try imageRequestHandler.perform([handPoseRequest])
            let observation = handPoseRequest.results?.first
            if observation == nil {
                msg = ""
            }
            else
            {
                msg = ""
                let displayTransform = session?.currentFrame?.displayTransform(for: UIInterfaceOrientation.portrait, viewportSize: UIScreen.main.bounds.size)
         
                let points = try observation!.recognizedPoints(.all)
                for (key,value) in points
                {
                    guard let name = nameConversions[key] else {
                        continue
                    }
                    
                    let p = CGPoint(x:value.location.x,y:1-value.location.y)
                    let pos = p.applying(displayTransform!)
                    msg += "\(name):\(pos.x)|\(1-pos.y)|\(value.confidence);"
                }
            }
        }
        catch {
            print(error)
        }
        return msg
      
    }
}
