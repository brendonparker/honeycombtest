## Create a role for the lambda to use
```
aws iam create-role --role-name lambda-ex --assume-role-policy-document '{"Version": "2012-10-17","Statement": [{ "Effect": "Allow", "Principal": {"Service": "lambda.amazonaws.com"}, "Action": "sts:AssumeRole"}]}'
```

## To deploy
1. Clean up previous deploys
```
rm -rf ./publish
```

2. dotnet publish
```
dotnet publish src/honeycombtest.csproj -c Release  -o ./publish
```

3. Zip that publish folder's contents up

4. Upload to S3:
```
aws s3 cp ./publish.zip s3://YOUR_BUCKET_NAME
```

5. Update the S3 json files with YOUR_BUCKET_NAME 

6. Create function 
```
aws lambda update-function-code --cli-input-json file://aws/update-function-code.json
```

7. Update function
```
aws lambda create-function --cli-input-json file://aws/input.json
```