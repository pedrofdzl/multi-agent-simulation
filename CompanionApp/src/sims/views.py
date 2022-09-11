from django.shortcuts import render, get_object_or_404, redirect
from django.urls import reverse

from .models import Agent, Simulation

def index(request):
    context = {}

    sims = Simulation.objects.all().order_by('-carrieout_date')[:5]
    # print(sims)
    context['sims'] = sims

    return render(request, 'index.html', context)


def sims_list(request):
    context = {}

    sims = Simulation.objects.all()
    context['sims'] = sims

    return render(request, 'sims_list.html', context)


def sim_detail(request, id):
    context = {}

    sim = get_object_or_404(Simulation, pk=id)
    context['sim'] = sim

    return render(request, 'sim_detail.html', context)


def sim_delete(request, id):
    context = {}

    sim = get_object_or_404(Simulation, pk=id)
    context['sim'] = sim

    if request.method == 'POST':
        sim.delete()
        return redirect(reverse('sims_list'))

    return render(request, 'sim_confirm_delete.html', context)


def agent_detail(request, id):
    context = {}

    agent = get_object_or_404(Agent, pk=id)
    context['agent'] = agent

    return render(request, 'agent_detail.html', context)

