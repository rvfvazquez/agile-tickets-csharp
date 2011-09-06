﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AgileTickets.Web.Models;
using AgileTickets.Web.Repositorios;
using NHibernate;
using AgileTickets.Web.Infra.Database;

namespace AgileTickets.Web.Controllers
{
    public class SessoesController : Controller
    {
        private Agenda agenda;
        private ISession session;
        public SessoesController(Agenda agenda, ISession session)
        {
            this.agenda = agenda;
            this.session = session;
        }
        public ActionResult Criar(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Index()
        {
            return View(agenda.ProximasSessoes(10));
        }

        public ActionResult Exibir(int sessaoId)
        {
            return View(agenda.Sessao(sessaoId));
        }

        [RequiresTransaction]
        public ActionResult Reservar(int sessaoId, int quantidade)
        {
            Sessao sessao = agenda.Sessao(sessaoId);

            if (!sessao.PodeReservar(quantidade))
            {
                ViewBag.MensagemDeErro = "Você não pode reservar mais do que " + sessao.IngressosDisponiveis + " ingressos!";
                return RedirectToAction("Exibir", new { id = sessaoId });
            }

            sessao.Reserva(quantidade);
            agenda.Atualiza(sessao);

            return RedirectToAction("Index");
        }

        [RequiresTransaction]
        public ActionResult Salvar(int id, DateTime inicio, DateTime fim, Periodicidade periodicidade)
        {
            Espetaculo espetaculo = agenda.Espetaculo(id);
            List<Sessao> novasSessoes = espetaculo.CriaSessoes(inicio, fim, periodicidade);

            agenda.Agende(novasSessoes);

            return RedirectToAction("Index", "Sessoes");
        }

    }
}
