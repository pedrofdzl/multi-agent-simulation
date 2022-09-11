from rest_framework import serializers
from rest_framework.validators import UniqueValidator, UniqueTogetherValidator
from .models import Agent, Simulation
from datetime import date


class AgentSerializer(serializers.Serializer):
    pk = serializers.IntegerField(read_only=True)
    agent_id = serializers.IntegerField()
    simulation_id = serializers.PrimaryKeyRelatedField(source='simulation', queryset=Simulation.objects.all())
    sim_track_id = serializers.ReadOnlyField(source='simulation.track_id')
    colisions = serializers.IntegerField()
    total_time = serializers.IntegerField()
    finished = serializers.BooleanField()


    class Meta:
        validators = [
            UniqueTogetherValidator(
                queryset=Agent.objects.all(),
                fields=['agent_id', 'simulation_id']
            )
        ]


    def create(self, validated_data):
        sim = validated_data.pop('sim')
        agent = Agent(**validated_data)
        agent.simulation = sim
        agent.save()
        return agent


    def updated(self, instance, validated_data):
        instance.agent_id = validated_data.get('agent_id', instance.agent_id)
        instance.simulation = validated_data.get('simulation', instance.simulation)
        instance.colisions = validated_data.get('colisions', instance.colisions)
        instance.total_time = validated_data.get('total_time', instance.total_time)
        instance.finished = validated_data.get('finished', instance.finished)

        instance.save()
        return instance



class SimulationSerializer(serializers.Serializer):
    pk = serializers.IntegerField(read_only=True)
    track_id = serializers.CharField(max_length=255, validators=[UniqueValidator(Simulation.objects.all(), 'Id already exists')])
    carrieout_date = serializers.DateField(default=date.today) # 'yyyy-mm-dd'
    agent_count = serializers.IntegerField(read_only=True)
    finished_agents = serializers.IntegerField(read_only=True)
    average_agent_time = serializers.IntegerField(read_only=True)
    total_colisions = serializers.IntegerField(read_only=True)
    # agents = serializers.RelatedField(many=True, read_only=True)
    agents = AgentSerializer(many=True, read_only=True)

    def create(self, validated_data):
        return Simulation.objects.create(**validated_data)

    
    def update(self, instance, validated_data):
        instance.track_id = validated_data.get('track_id', instance.track_id)
        instance.carrieout_date = validated_data.get('carrieout_date', instance.track_id)

        instance.save()

        return instance


class SimCreateSerializer(serializers.Serializer):
    pk = serializers.IntegerField(read_only=True)
    track_id = serializers.CharField(max_length=255, read_only=True)
