#!/bin/bash

filename='Infinni.Deployer.ubuntu.zip'

wget --quiet --method=GET --user=$1 --ask-password 'http://teamcity.infinnity.ru/app/rest/builds/buildType:%28id:InfinniDeployer_Publish%29,status:SUCCESS/artifacts/content/Infinni.Deployer.ubuntu.zip'

rm -rf Infinni.Deployer

unzip Infinni.Deployer.ubuntu.zip -d Infinni.Deployer

rm Infinni.Deployer.ubuntu.zip

chmod +x Infinni.Deployer/deployer