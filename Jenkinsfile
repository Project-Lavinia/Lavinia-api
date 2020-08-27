def testPassed = true

pipeline {
  agent any
  environment {
    ARTIFACT = "artifact.zip"
  }
  stages {
    stage('Build') {
      steps {
        sh "dotnet restore"
        sh "dotnet build --configuration Release"
        script {
          try {
            sh "dotnet test --logger \"trx;LogFileName=TestResults.trx\""
          } catch (ex) {
            unstable('Some tests failed')
            testPassed = false
          }
        }
        mstest testResultsFile:"**/*.trx", keepLongStdio: true
        sh "cd Lavinia-api/bin/Release/netcoreapp3.1; zip -r ${WORKSPACE}/${ARTIFACT} *; cd ${WORKSPACE}"
        archiveArtifacts artifacts: ARTIFACT
      }
    }

    stage('Deploy') {
      when { (tag "*.*.*") && testPassed }
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
      when { (tag "*.*.*") && testPassed }
      environment {
        GITHUB_TOKEN = credentials('jenkins_release_token')
        REPOSITORY = "Project-Lavinia/Lavinia-api"
      }
      steps {
        publishArtifact()
      }
    }
  }
}

void publishArtifact() {
    def current_tag = sh(returnStdout: true, script: "git tag --sort version:refname | tail -1").trim()
    def release_data = sh(returnStdout: true, script: "curl https://api.github.com/repos/${REPOSITORY}/releases/tags/${current_tag}").trim()
    def release_json = readJSON text: release_data
    def release_id = release_json.id
    sh "curl -XPOST -H 'Authorization:token ${GITHUB_TOKEN}' -H 'Content-Type:application/octet-stream' --data-binary @${ARTIFACT} https://uploads.github.com/repos/${REPOSITORY}/releases/${release_id}/assets?name=${ARTIFACT}"
}