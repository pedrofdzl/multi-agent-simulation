from rest_framework.response import Response
from rest_framework.decorators import api_view
from rest_framework.parsers import JSONParser
from rest_framework import status

from sims.serializers import SimulationSerializer, AgentSerializer, SimCreateSerializer
from sims.models import Agent, Simulation

from datetime import date


@api_view(['GET', 'POST'])
def sim_list(request):
    if request.method == 'GET':
        sims = Simulation.objects.prefetch_related('agents')
        serializer = SimulationSerializer(sims, many=True)

        return Response(serializer.data)

    elif request.method == 'POST':
        
            sim = Simulation.objects.create(carrieout_date=date.today())
            sim.track_id = f"SIM {sim.pk}"
            sim.save()

            serializer = SimCreateSerializer(sim)
        
            return Response(serializer.data, status=status.HTTP_201_CREATED)


@api_view(['GET', 'PUT', 'DELETE'])
def sim_detail(request, pk):
    try:
        sim = Simulation.objects.get(pk=pk)
    except Simulation.DoesNotExist:
        return Response(status=status.HTTP_404_NOT_FOUND)

    if request.method == 'GET':
        serializer = SimulationSerializer(sim)
        return Response(serializer.data, status=status.HTTP_200_OK)

    elif request.method == 'PUT':
        data = JSONParser().parse(request)
        serializer = SimulationSerializer(sim, data=data)

        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data)
        
        return Response(serializer.errors,status=status.HTTP_400_BAD_REQUEST)

    elif request.method == 'DELETE':
        sim.delete()
        return Response(status=status.HTTP_204_NO_CONTENT)



@api_view(['GET', 'POST'])
def agent(request, pk):
    try:
        sim = Simulation.objects.get(pk=pk)
    except Simulation.DoesNotExist:
        return Response(status=status.HTTP_404_NOT_FOUND)
    
    if request.method == 'GET':
        agents = Agent.objects.filter(simulation=sim)
        serializer = AgentSerializer(agents, many=True)

        return Response(serializer.data, status=status.HTTP_200_OK)

    elif request.method == 'POST':
        data = JSONParser().parse(request)
        

        if type(data) == list:
            for item in data:
                item['simulation_id'] = sim.pk
            serializer = AgentSerializer(data=data, many=True)
        else:
            data['simulation_id'] = sim.pk
            serializer = AgentSerializer(data=data)

        if serializer.is_valid():
            serializer.save(sim=sim)
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
