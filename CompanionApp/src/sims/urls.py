from django.urls import path
from . import views


urlpatterns = [
    path('', views.index, name='index'),
    path('sims/', views.sims_list, name='sims_list'),
    path('sim/<int:id>/', views.sim_detail, name='sim_detail'),
    path('sim/<int:id>/delete/', views.sim_delete, name='sim_delete'),

    path('agent/<int:id>/', views.agent_detail, name='agent_detail')

]