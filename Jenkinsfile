pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh "dotnet restore"
        sh "dotnet build --configuration Release"
        sh "cd Lavinia-api/bin/Release/netcoreapp3.1; zip -r ../artifact.zip *; cd ${WORKSPACE}"
        archiveArtifacts artifacts: 'artifact.zip'
      }
    }

    stage('Deploy') {
      when { tag "*.*.*" }
      steps {
        ansiblePlaybook(
          playbook: '/storage/api_deploy.yaml',
          credentialsId: 'ansible_key',
          inventory: '/storage/hosts',
          disableHostKeyChecking: true
        )
      }
    }

    stage('Release') {
      when { tag "*.*.*" }
      environment {
        GITHUB_TOKEN = credentials('jenkins_release_token')
      }
      steps {
        publishArtifact()
      }
    }
  }
}

void publishArtifact() {
    def current_tag = sh(returnStdout: true, script: "git tag --sort version:refname | tail -1").trim()
    sh "echo ${current_tag}"
    def release_data = sh(returnStdout: true, script: "curl https://api.github.com/repos/Project-Lavinia/Lavinia-client/releases/tags/${current_tag}").trim()
    def release_json = readJSON text: release_data
    sh "echo ${release_json.id}"
    def release_id = release_json.id
    sh "echo ${release_id}"
    def release_id_str = sh(returnStdout: true, script: "echo ${release_id}").trim()
    sh "echo ${release_id_str}"
    sh "curl -XPOST -H 'Authorization:token ${GITHUB_TOKEN}' -H 'Content-Type:application/octet-stream' --data-binary @artifact.zip https://uploads.github.com/repos/Project-Lavinia/Lavinia-client/releases/${release_id_str}/assets?name=artifact.zip"
}