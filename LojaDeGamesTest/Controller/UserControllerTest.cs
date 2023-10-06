using FluentAssertions;
using LojaDeGames.Model;
using LojaDeGamesTest.Factory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions.Ordering;

namespace LojaDeGamesTest.Controller
{
    public class UserControllerTest : IClassFixture<WebAppFactory>
    {

        protected readonly WebAppFactory _factory;
        protected HttpClient _client;

        private readonly dynamic token;
        private string Id { get; set; } = string.Empty;

        public UserControllerTest (WebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            token = GetToken();
        }

        private static dynamic GetToken()
        {
            dynamic data = new ExpandoObject();
            data.sub = "root@root.com";
            return data;
        }

        [Fact, Order(1)]
        public async Task DeveCriarUmUsuario()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                {"nome", "Robson" },
                {"usuario", "robson@email.com" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            resposta.EnsureSuccessStatusCode();

            resposta.StatusCode.Should().Be(HttpStatusCode.Created);
        }


        [Fact, Order(2)]
        public async Task DeveDarErroEmail()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                {"nome", "Robson" },
                {"usuario", "robson-email.com" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            

            resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact, Order(3)]
        public async Task NaoDeveCriarUsuarioDuplicado()
        {
            var novoUsuario = new Dictionary<string, string>()
            {
                {"nome", "Robson" },
                {"usuario", "robson@email.com" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);
            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);



            resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact, Order(4)]
        public async Task NaoAutorizado()
        {
            //_client.SetFakeBearerToken("lalsdjasjndsajfsadfasdfksla");


            var resposta = await _client.GetAsync("/usuarios/listarTodos");



            //resposta.StatusCode.Should().Be(HttpStatusCode.OK);



            resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, Order(5)]
        public async Task DeveListarUsuarios()
        {

            _client.SetFakeBearerToken((object) token);

            
            var resposta = await _client.GetAsync("/usuarios/listarTodos");



            resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact, Order(6)]
        public async Task DeveAtualizarUmUsuario()
        {

            

            var novoUsuario = new Dictionary<string, string>
            {
                {"nome", "Robson" },
                {"usuario", "robson@email.com" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            var usuarioJson = JsonConvert.SerializeObject(novoUsuario);
            var corpoRequisicao = new StringContent(usuarioJson, Encoding.UTF8, "application/json");

            var resposta = await _client.PostAsync("/usuarios/cadastrar", corpoRequisicao);

            var corpoRespostaPost = await resposta.Content.ReadFromJsonAsync<User>();

            if(corpoRespostaPost is not null)
            {
                Id = corpoRespostaPost.Id.ToString();
            }

            var atualizarUsuario = new Dictionary<string, string>
            {
                {"Id" , Id },
                {"nome", "Robson" },
                {"usuario", "robson@email.com" },
                {"senha", "123456789" },
                {"foto", "" }
            };

            _client.SetFakeBearerToken((object)token);

            var atualizarUsuarioJson = JsonConvert.SerializeObject(atualizarUsuario);
            var corpoRequisicaoAtualizar = new StringContent(atualizarUsuarioJson, Encoding.UTF8, "application/json");

            
            var respostaput = await _client.PutAsync("/usuarios/atualizar", corpoRequisicaoAtualizar);

            respostaput.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
