package com.kcci.hp.common.util;

import java.nio.ByteBuffer;
import java.util.List;

import com.amazonaws.auth.AWSCredentialsProvider;
import com.amazonaws.auth.AWSStaticCredentialsProvider;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.rekognition.AmazonRekognition;
import com.amazonaws.services.rekognition.AmazonRekognitionClientBuilder;
import com.amazonaws.services.rekognition.model.AmazonRekognitionException;
import com.amazonaws.services.rekognition.model.Attribute;
import com.amazonaws.services.rekognition.model.BoundingBox;
import com.amazonaws.services.rekognition.model.CompareFacesMatch;
import com.amazonaws.services.rekognition.model.CompareFacesRequest;
import com.amazonaws.services.rekognition.model.CompareFacesResult;
import com.amazonaws.services.rekognition.model.ComparedFace;
import com.amazonaws.services.rekognition.model.DetectFacesRequest;
import com.amazonaws.services.rekognition.model.DetectFacesResult;
import com.amazonaws.services.rekognition.model.FaceDetail;
import com.amazonaws.services.rekognition.model.Image;


public class PhotoUtil  {
	
	public static String accessKey = "";
	public static String secretKey = "";
	public static Float similarityThreshold = 70F;
	
	public static boolean rekognition_detect_face(byte[] source) {
		
		boolean photoCheck = false;
		/*
		BasicAWSCredentials creds = new BasicAWSCredentials(accessKey, secretKey);
	    AWSStaticCredentialsProvider provider = new 
	    AWSStaticCredentialsProvider(creds);
		*/
		 AWSCredentialsProvider credProvider = new AWSStaticCredentialsProvider(
		            new BasicAWSCredentials(accessKey, secretKey));
		
	    AmazonRekognition ar =AmazonRekognitionClientBuilder.standard()
	    .withCredentials(credProvider)
	    .withRegion(Regions.AP_NORTHEAST_2)
	    .build();
	    try {
	    	ByteBuffer imageBytes;
	        imageBytes = ByteBuffer.wrap(source);
		    
		    DetectFacesRequest request  = new DetectFacesRequest()
		    													.withImage(new Image()
		    															 .withBytes(imageBytes))
		    													.withAttributes(Attribute.ALL);

	  
	         DetectFacesResult result = ar.detectFaces(request );
	         List < FaceDetail > faceDetails = result.getFaceDetails();

	         for (FaceDetail face: faceDetails) {
	            if (request.getAttributes().contains("ALL")) {
	            	//사람 사진일 확률
	               float confidence =  face.getConfidence();	            	
					/*
					 * AgeRange ageRange = face.getAgeRange(); System.out.println("측정된 나이는  " +
					 * ageRange.getLow().toString() + "세에서  " + ageRange.getHigh().toString() +
					 * " 입니다.");
					 */
	               if(confidence > similarityThreshold) {
	            	   photoCheck = true;
	               }
	            } else { // non-default attributes have null values.
	               System.out.println("");
	            }

	            //ObjectMapper objectMapper = new ObjectMapper();
	            //System.out.println(objectMapper.writerWithDefaultPrettyPrinter().writeValueAsString(face));
	         }

	     } catch (AmazonRekognitionException e) {
	         e.printStackTrace();
		} 
	    return photoCheck;
	}
	
public static boolean rekognition_compare_face(byte[] sourceImage, byte[] targetImage) {
		
		boolean photoCheck = false;
    
	    ByteBuffer sourceImageBytes = ByteBuffer.wrap(sourceImage);	       
	    ByteBuffer targetImageBytes = ByteBuffer.wrap(targetImage);
		
	    AWSCredentialsProvider credProvider = new AWSStaticCredentialsProvider(
	            new BasicAWSCredentials(accessKey, secretKey));

	    AmazonRekognition ar =AmazonRekognitionClientBuilder.standard()
	    .withCredentials(credProvider)
	    .withRegion(Regions.AP_NORTHEAST_2)
	    .build();
	    
	    try {
	    	   Image source=new Image()
	    	            .withBytes(sourceImageBytes);
    	       Image target=new Image()
    	            .withBytes(targetImageBytes);

    	       CompareFacesRequest request = new CompareFacesRequest()
    	               .withSourceImage(source)
    	               .withTargetImage(target)
    	               .withSimilarityThreshold(similarityThreshold);
    	       
    	       CompareFacesResult compareFacesResult=ar.compareFaces(request);
  
    	       List <CompareFacesMatch> faceDetails = compareFacesResult.getFaceMatches();
    	       for (CompareFacesMatch match: faceDetails){
    	         ComparedFace face= match.getFace();
    	         BoundingBox position = face.getBoundingBox();
    	         System.out.println("Face at " + position.getLeft().toString()
    	               + " " + position.getTop()
    	               + " matches with " + face.getConfidence().toString()
    	               + "% confidence.");
    	         
    	         photoCheck = true;
    	       }
    	       
    	       /*
    	       List<ComparedFace> uncompared = compareFacesResult.getUnmatchedFaces();

    	       System.out.println("There was " + uncompared.size()
    	            + " face(s) that did not match");
    	       System.out.println("Source image rotation: " + compareFacesResult.getSourceImageOrientationCorrection());
    	       System.out.println("target image rotation: " + compareFacesResult.getTargetImageOrientationCorrection());
	    	   */
	     } catch (AmazonRekognitionException e) {
	         e.printStackTrace();
			// TODO Auto-generated catch block
			e.printStackTrace();
		} 
	    return photoCheck;
	}
}
