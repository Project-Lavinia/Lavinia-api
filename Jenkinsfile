pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet build --configuration Release'
      }
    }

    stage('Deploy') {
      when{
        branch 'master'
      }
      steps {
        ansiblePlaybook('/storage/web_deploy.yaml') {
          credentialsId('ansible_key')
          inventoryPath('/storage/hosts')
        }
      }
    }
  }
}