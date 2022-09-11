from django.db import models
from datetime import date

class Simulation(models.Model):
    track_id = models.CharField('Tracking ID',max_length=255, unique=True, null=True, blank=True)
    carrieout_date = models.DateField('Carrie Out Date', default=date.today)


    class Meta:
        verbose_name = 'Simulation'
        verbose_name_plural = 'Simulations'


    def __str__(self) ->str:
        return f'{self.track_id}'


    def __repr__(self):
        return f'<Simulation> {self.track_id}'


    @property
    def agent_count(self):
        total = self.agents.all().count()
        return total

    
    @property
    def finished_agents(self):
        agents = self.agents.filter(finished=True)

        total = agents.count()
        return total

    
    @property
    def average_agent_time(self):
        agents = self.agents.filter(finished=True)

        try:
            total = sum([agent.total_time for agent in agents]) / agents.count()
            return total
        except ZeroDivisionError:
            return 0
    

    @property
    def total_colisions(self):
        agents = self.agents.all()

        total = sum([agent.colisions for agent in agents])
        return total

class Agent(models.Model):
    agent_id = models.IntegerField('Agent Id')
    simulation = models.ForeignKey(Simulation, on_delete=models.CASCADE, related_name='agents')
    colisions = models.IntegerField('Colisions')
    total_time = models.IntegerField('total Time') # in seconds
    finished = models.BooleanField('Finished', default=False)

    class Meta:
        verbose_name = 'Agent'
        verbose_name_plural = 'Agents'

        constraints = [
            models.UniqueConstraint(fields=['agent_id', 'simulation'], name='unique_sim_agent')
        ]


    def __str__(self):
        return f'{self.simulation.track_id} Agent: {self.agent_id}'

    def __repr__(self):
        return f'<Agent> {self.simulation.tracking_id} {self.agent_id}'


