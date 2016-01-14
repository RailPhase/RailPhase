del RailPhase.*.nupkg
nuget pack -Prop Configuration=Release
nuget push RailPhase.*.nupkg
