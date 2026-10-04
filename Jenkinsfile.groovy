pipeline {
    agent any

    stages {
        stage('Restore') {
            steps {
                dir('backend') {
                    bat 'dotnet restore'
                }
            }
        }

        stage('Build') {
            steps {
                dir('backend') {
                    bat 'dotnet build --no-restore -c Release'
                }
            }
        }

        stage('Dependências vulneráveis') {
            steps {
                dir('backend') {
                    bat 'dotnet list package --vulnerable --include-transitive'
                }
            }
        }
    }
}