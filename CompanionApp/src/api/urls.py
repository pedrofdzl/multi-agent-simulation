from django.urls import path
from .api_views import sims_views

urlpatterns = [
    path('sims/', sims_views.sim_list),
    path('sims/<int:pk>/', sims_views.sim_detail),

    path('agents/<int:pk>/', sims_views.agent),
]